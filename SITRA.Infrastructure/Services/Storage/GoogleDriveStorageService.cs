using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.Services.Storage
{
    public class GoogleDriveStorageService : IDgiiStorageService, IFileStorageService
    {
        private readonly DgiiSettings _settings;
        private readonly ILogger<GoogleDriveStorageService> _logger;
        private readonly IConfiguration _configuration;
        private DriveService? _driveService;
        private string? _rootFolderId;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private readonly ConcurrentDictionary<string, string> _folderCache = new(StringComparer.OrdinalIgnoreCase);

        public GoogleDriveStorageService(
            IOptions<DgiiSettings> options,
            IConfiguration configuration,
            ILogger<GoogleDriveStorageService> logger)
        {
            _settings = options.Value;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<DriveService> GetDriveServiceAsync()
        {
            if (_driveService != null) return _driveService;

            await _initLock.WaitAsync();
            try
            {
                if (_driveService != null) return _driveService;

                string credentialsPath = !string.IsNullOrWhiteSpace(_settings.GoogleDriveCredentialsPath)
                    ? _settings.GoogleDriveCredentialsPath
                    : _configuration["DgiiTest:GoogleDriveCredentialsPath"] 
                      ?? _configuration["Dgii:GoogleDriveCredentialsPath"] 
                      ?? "reliable-sight-510515-p2-8ac840488d3f.json";

                string resolvedCredentialsPath = ResolveFilePath(credentialsPath);
                if (!File.Exists(resolvedCredentialsPath))
                {
                    throw new FileNotFoundException($"Google Drive credentials file not found at: {resolvedCredentialsPath}");
                }

                _rootFolderId = !string.IsNullOrWhiteSpace(_settings.GoogleDriveFolderId)
                    ? _settings.GoogleDriveFolderId
                    : _configuration["DgiiTest:GoogleDriveFolderId"] 
                      ?? _configuration["Dgii:GoogleDriveFolderId"] 
                      ?? "1Y67Zptg_8nHlPr7BvB2GsuLqIZZ3-geU";

                if (string.IsNullOrWhiteSpace(_rootFolderId))
                {
                    throw new InvalidOperationException("Google Drive Root Folder ID is not configured.");
                }

                _logger.LogInformation("Initializing Google Drive service with credentials from {Path} and root folder {FolderId}", 
                    resolvedCredentialsPath, _rootFolderId);

                string jsonContent = await File.ReadAllTextAsync(resolvedCredentialsPath);
                if (jsonContent.Contains("\"installed\"") || jsonContent.Contains("\"web\""))
                {
                    // OAuth 2.0 Client Credentials
                    string tokenFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SITRA", "GoogleDriveToken");
                    if (!Directory.Exists(tokenFolder)) Directory.CreateDirectory(tokenFolder);

                    using var stream = new FileStream(resolvedCredentialsPath, FileMode.Open, FileAccess.Read);
                    var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

                    var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        clientSecrets,
                        new[] { DriveService.Scope.Drive },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(tokenFolder, true),
                        new BrowserCodeReceiver());

                    _driveService = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "SITRA"
                    });
                }
                else
                {
                    // Service Account
                    GoogleCredential credential;
                    using (var stream = new FileStream(resolvedCredentialsPath, FileMode.Open, FileAccess.Read))
                    {
                        credential = GoogleCredential.FromStream(stream)
                            .CreateScoped(DriveService.Scope.Drive);
                    }

                    _driveService = new DriveService(new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "SITRA"
                    });
                }

                return _driveService;
            }
            finally
            {
                _initLock.Release();
            }
        }

        #region Folder Navigation Helpers

        private async Task<string> GetOrCreateSubfolderAsync(string parentFolderId, string folderName)
        {
            string cacheKey = $"{parentFolderId}/{folderName}";
            if (_folderCache.TryGetValue(cacheKey, out var cachedId))
                return cachedId;

            var drive = await GetDriveServiceAsync();
            string escapedName = folderName.Replace("'", "\\'");

            var listReq = drive.Files.List();
            listReq.Q = $"'{parentFolderId}' in parents and name = '{escapedName}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";
            listReq.Fields = "files(id, name)";
            var listResult = await listReq.ExecuteAsync();

            var existingFolder = listResult.Files?.FirstOrDefault();
            if (existingFolder != null)
            {
                _folderCache[cacheKey] = existingFolder.Id;
                return existingFolder.Id;
            }

            var folderMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentFolderId }
            };

            var createReq = drive.Files.Create(folderMeta);
            createReq.Fields = "id";
            var created = await createReq.ExecuteAsync();

            _folderCache[cacheKey] = created.Id;
            return created.Id;
        }

        private async Task<string> ResolveFolderPathAsync(string relativePath)
        {
            var drive = await GetDriveServiceAsync();
            string currentFolderId = _rootFolderId!;

            var parts = relativePath.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                currentFolderId = await GetOrCreateSubfolderAsync(currentFolderId, part);
            }

            return currentFolderId;
        }

        #endregion

        #region IDgiiStorageService Implementation

        public async Task<string> SaveDocumentXmlAsync(string xml, string rnc, string docType, string fileName)
        {
            var drive = await GetDriveServiceAsync();
            string folderPath = $"Invoices/{rnc}/{docType}";
            string folderId = await ResolveFolderPathAsync(folderPath);

            // Check if file already exists in target folder
            var listReq = drive.Files.List();
            string escapedName = fileName.Replace("'", "\\'");
            listReq.Q = $"'{folderId}' in parents and name = '{escapedName}' and trashed = false";
            listReq.Fields = "files(id, name)";
            var listResult = await listReq.ExecuteAsync();
            var existingFile = listResult.Files?.FirstOrDefault();

            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            using var stream = new MemoryStream(bytes);

            if (existingFile != null)
            {
                var updateReq = drive.Files.Update(new Google.Apis.Drive.v3.Data.File(), existingFile.Id, stream, "application/xml");
                updateReq.Fields = "id, webViewLink";
                await updateReq.UploadAsync();
                return $"https://drive.google.com/file/d/{existingFile.Id}/view";
            }
            else
            {
                var fileMeta = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    Parents = new List<string> { folderId }
                };

                var createReq = drive.Files.Create(fileMeta, stream, "application/xml");
                createReq.Fields = "id, webViewLink";
                var progress = await createReq.UploadAsync();
                if (progress.Status == UploadStatus.Failed)
                {
                    throw progress.Exception ?? new InvalidOperationException($"Error uploading XML {fileName} to Google Drive.");
                }

                string fileId = createReq.ResponseBody.Id;
                return $"https://drive.google.com/file/d/{fileId}/view";
            }
        }

        public async Task<string?> ReadDocumentXmlFromUrlAsync(string urlOrPath)
        {
            if (string.IsNullOrWhiteSpace(urlOrPath)) return null;

            try
            {
                string? fileId = ExtractFileId(urlOrPath);
                if (!string.IsNullOrEmpty(fileId))
                {
                    var drive = await GetDriveServiceAsync();
                    using var ms = new MemoryStream();
                    var getReq = drive.Files.Get(fileId);
                    await getReq.DownloadAsync(ms);
                    if (ms.Length > 0)
                    {
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }

                // Fallback: If it's a standard URL, try HTTP GET
                if (urlOrPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    urlOrPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    using var httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    var res = await httpClient.GetAsync(urlOrPath);
                    if (res.IsSuccessStatusCode)
                    {
                        return await res.Content.ReadAsStringAsync();
                    }
                }
                else if (File.Exists(urlOrPath))
                {
                    return await File.ReadAllTextAsync(urlOrPath);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read document XML from Google Drive for {UrlOrPath}", urlOrPath);
                return null;
            }
        }

        public async Task<string?> ReadDocumentXmlAsync(string rnc, string docType, string fileName)
        {
            try
            {
                var drive = await GetDriveServiceAsync();
                string folderPath = $"Invoices/{rnc}/{docType}";
                string folderId = await ResolveFolderPathAsync(folderPath);

                var listReq = drive.Files.List();
                string escapedName = fileName.Replace("'", "\\'");
                listReq.Q = $"'{folderId}' in parents and name = '{escapedName}' and trashed = false";
                listReq.Fields = "files(id, name)";
                var listResult = await listReq.ExecuteAsync();

                var file = listResult.Files?.FirstOrDefault();
                if (file == null) return null;

                using var ms = new MemoryStream();
                await drive.Files.Get(file.Id).DownloadAsync(ms);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read document XML {Rnc}/{DocType}/{FileName} from Google Drive", rnc, docType, fileName);
                return null;
            }
        }

        public async Task<byte[]> ReadCertificateAsync()
        {
            string certPath = ResolveFilePath(_settings.CertificatePath);
            if (File.Exists(certPath))
            {
                return await File.ReadAllBytesAsync(certPath);
            }

            // Fallback: try reading from Drive if path is a URL or file ID
            string? fileId = ExtractFileId(_settings.CertificatePath);
            if (!string.IsNullOrEmpty(fileId))
            {
                var drive = await GetDriveServiceAsync();
                using var ms = new MemoryStream();
                await drive.Files.Get(fileId).DownloadAsync(ms);
                return ms.ToArray();
            }

            throw new FileNotFoundException($"Certificate not found at: {certPath}");
        }

        public async Task<byte[]> ReadCertificateFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Certificate URL cannot be empty.", nameof(url));

            string? fileId = ExtractFileId(url);
            if (!string.IsNullOrEmpty(fileId))
            {
                var drive = await GetDriveServiceAsync();
                using var ms = new MemoryStream();
                await drive.Files.Get(fileId).DownloadAsync(ms);
                return ms.ToArray();
            }

            if (File.Exists(url))
            {
                return await File.ReadAllBytesAsync(url);
            }

            throw new FileNotFoundException($"Could not load certificate from URL or path: {url}");
        }

        public async Task<Stream> ReadFileStreamAsync(string relativePath)
        {
            string? fileId = ExtractFileId(relativePath);
            var drive = await GetDriveServiceAsync();

            if (!string.IsNullOrEmpty(fileId))
            {
                var ms = new MemoryStream();
                await drive.Files.Get(fileId).DownloadAsync(ms);
                ms.Position = 0;
                return ms;
            }

            string localPath = ResolveFilePath(relativePath);
            if (File.Exists(localPath))
            {
                return File.OpenRead(localPath);
            }

            throw new FileNotFoundException($"File not found at: {relativePath}");
        }

        public async Task<string[]> GetSchemaFilesAsync()
        {
            var drive = await GetDriveServiceAsync();
            string schemasFolderId = await ResolveFolderPathAsync("Schemas");

            var listReq = drive.Files.List();
            listReq.Q = $"'{schemasFolderId}' in parents and trashed = false";
            listReq.Fields = "files(id, name)";
            var result = await listReq.ExecuteAsync();

            if (result.Files != null && result.Files.Any())
            {
                return result.Files
                    .Where(f => f.Name.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Name)
                    .ToArray();
            }

            // Fallback to local
            string localDir = ResolveFilePath(_settings.SchemasPath);
            if (Directory.Exists(localDir))
            {
                return Directory.GetFiles(localDir, "*.xsd").Select(Path.GetFileName).ToArray()!;
            }

            return Array.Empty<string>();
        }

        public async Task<Stream> ReadSchemaStreamAsync(string schemaFileName)
        {
            var drive = await GetDriveServiceAsync();
            string schemasFolderId = await ResolveFolderPathAsync("Schemas");

            var listReq = drive.Files.List();
            string escapedName = schemaFileName.Replace("'", "\\'");
            listReq.Q = $"'{schemasFolderId}' in parents and name = '{escapedName}' and trashed = false";
            listReq.Fields = "files(id, name)";
            var result = await listReq.ExecuteAsync();

            var file = result.Files?.FirstOrDefault();
            if (file != null)
            {
                var ms = new MemoryStream();
                await drive.Files.Get(file.Id).DownloadAsync(ms);
                ms.Position = 0;
                return ms;
            }

            // Fallback to local
            string localPath = Path.Combine(ResolveFilePath(_settings.SchemasPath), schemaFileName);
            if (File.Exists(localPath))
            {
                return File.OpenRead(localPath);
            }

            throw new FileNotFoundException($"Schema file not found: {schemaFileName}");
        }

        public async Task EnsureSchemasUploadedAsync()
        {
            string localSchemasDir = ResolveFilePath(_settings.SchemasPath);
            if (!Directory.Exists(localSchemasDir))
            {
                _logger.LogWarning("EnsureSchemasUploadedAsync: Local schemas directory not found at '{Path}'.", localSchemasDir);
                return;
            }

            var drive = await GetDriveServiceAsync();
            string schemasFolderId = await ResolveFolderPathAsync("Schemas");

            var localFiles = Directory.GetFiles(localSchemasDir, "*.xsd");
            var listReq = drive.Files.List();
            listReq.Q = $"'{schemasFolderId}' in parents and trashed = false";
            listReq.Fields = "files(id, name)";
            var existingFiles = await listReq.ExecuteAsync();
            var existingNames = new HashSet<string>(existingFiles.Files?.Select(f => f.Name) ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            foreach (var localFile in localFiles)
            {
                string fileName = Path.GetFileName(localFile);
                if (existingNames.Contains(fileName))
                {
                    continue;
                }

                _logger.LogInformation("EnsureSchemasUploadedAsync: Uploading schema '{FileName}' to Google Drive.", fileName);
                using var fileStream = File.OpenRead(localFile);
                var fileMeta = new Google.Apis.Drive.v3.Data.File
                {
                    Name = fileName,
                    Parents = new List<string> { schemasFolderId }
                };

                var createReq = drive.Files.Create(fileMeta, fileStream, "application/xml");
                await createReq.UploadAsync();
                _logger.LogInformation("EnsureSchemasUploadedAsync: Uploaded schema '{FileName}' successfully.", fileName);
            }
        }

        public async Task<string> SaveCertificateAsync(IFormFile file, string rnc)
        {
            var drive = await GetDriveServiceAsync();
            string folderId = await ResolveFolderPathAsync($"Certificates/{rnc}");

            using var stream = file.OpenReadStream();
            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { folderId }
            };

            var createReq = drive.Files.Create(fileMeta, stream, file.ContentType ?? "application/octet-stream");
            createReq.Fields = "id";
            await createReq.UploadAsync();

            string fileId = createReq.ResponseBody.Id;
            return $"https://drive.google.com/file/d/{fileId}/view";
        }

        public async Task<string> SaveCompanyLogoAsync(IFormFile file, string rnc)
        {
            var drive = await GetDriveServiceAsync();
            string folderId = await ResolveFolderPathAsync($"Logos/{rnc}");

            var extension = Path.GetExtension(file.FileName);
            string fileName = $"{rnc}-logo{extension}";

            using var stream = file.OpenReadStream();
            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var createReq = drive.Files.Create(fileMeta, stream, string.IsNullOrWhiteSpace(file.ContentType) ? "image/png" : file.ContentType);
            createReq.Fields = "id";
            await createReq.UploadAsync();

            string fileId = createReq.ResponseBody.Id;
            await MakeFilePublicAsync(drive, fileId);

            return $"https://drive.google.com/thumbnail?id={fileId}&sz=w1000";
        }

        public async Task<string> SaveItemPictureAsync(IFormFile file, string rnc, Guid? itemId = null)
        {
            const long maxPictureSizeBytes = 300 * 1024; // 300 KB
            if (file.Length > maxPictureSizeBytes)
            {
                throw new InvalidOperationException($"La imagen supera el tamaño máximo permitido de 300 KB ({file.Length} bytes).");
            }

            var drive = await GetDriveServiceAsync();
            string folderId = await ResolveFolderPathAsync($"Items/{rnc}");

            var extension = Path.GetExtension(file.FileName);
            string fileIdName = itemId.HasValue ? $"{itemId.Value}_{Guid.NewGuid():N}" : Guid.NewGuid().ToString("N");
            string fileName = $"{fileIdName}{extension}";

            using var stream = file.OpenReadStream();
            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var createReq = drive.Files.Create(fileMeta, stream, string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType);
            createReq.Fields = "id";
            await createReq.UploadAsync();

            string uploadedId = createReq.ResponseBody.Id;
            await MakeFilePublicAsync(drive, uploadedId);

            return $"https://drive.google.com/thumbnail?id={uploadedId}&sz=w1000";
        }

        public async Task DeleteItemPictureAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            string? fileId = ExtractFileId(url);
            if (!string.IsNullOrEmpty(fileId))
            {
                try
                {
                    var drive = await GetDriveServiceAsync();
                    await drive.Files.Delete(fileId).ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete item picture {FileId} from Google Drive", fileId);
                }
            }
        }

        #endregion

        #region IFileStorageService Implementation

        public async Task<string> SaveAsync(string container, IFormFile file)
        {
            var drive = await GetDriveServiceAsync();
            string folderId = await ResolveFolderPathAsync(container);

            var extension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var fileMeta = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var createReq = drive.Files.Create(fileMeta, stream, file.ContentType ?? "application/octet-stream");
            createReq.Fields = "id";
            await createReq.UploadAsync();

            string fileId = createReq.ResponseBody.Id;
            await MakeFilePublicAsync(drive, fileId);

            return $"https://drive.google.com/thumbnail?id={fileId}&sz=w1000";
        }

        public async Task DeleteAsync(string? url, string container)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            string? fileId = ExtractFileId(url);
            if (!string.IsNullOrEmpty(fileId))
            {
                try
                {
                    var drive = await GetDriveServiceAsync();
                    await drive.Files.Delete(fileId).ExecuteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete file {FileId} from Google Drive", fileId);
                }
            }
        }

        #endregion

        #region Helpers

        private static async Task MakeFilePublicAsync(DriveService drive, string fileId)
        {
            try
            {
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                await drive.Permissions.Create(permission, fileId).ExecuteAsync();
            }
            catch
            {
                // Non-fatal if organization restricts sharing outside domain
            }
        }

        private static string? ExtractFileId(string urlOrPath)
        {
            if (string.IsNullOrWhiteSpace(urlOrPath)) return null;

            // Pattern: /file/d/{id}/view or /d/{id}
            var match = Regex.Match(urlOrPath, @"/d/([a-zA-Z0-9_-]+)");
            if (match.Success) return match.Groups[1].Value;

            // Pattern: id={id}
            match = Regex.Match(urlOrPath, @"[?&]id=([a-zA-Z0-9_-]+)");
            if (match.Success) return match.Groups[1].Value;

            // Direct ID (Google Drive IDs are alphanumeric strings usually 28-50 chars)
            if (!urlOrPath.Contains('/') && !urlOrPath.Contains('\\') && urlOrPath.Length >= 25 && urlOrPath.Length <= 55)
            {
                return urlOrPath.Trim();
            }

            return null;
        }

        private static string ResolveFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            if (Path.IsPathRooted(path)) return path;

            string baseAppPath = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(baseAppPath) || Directory.Exists(baseAppPath))
            {
                return baseAppPath;
            }

            string currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            if (File.Exists(currentDirPath) || Directory.Exists(currentDirPath))
            {
                return currentDirPath;
            }

            return baseAppPath;
        }

        #endregion
    }

    public class BrowserCodeReceiver : ICodeReceiver
    {
        private readonly LocalServerCodeReceiver _inner = new();

        public string RedirectUri => _inner.RedirectUri;

        public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url, CancellationToken taskCancellationToken)
        {
            string authUrl = url.Build().ToString();
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Ignore if process start fails
            }

            Console.WriteLine($"\n=======================================================\nAUTORIZACION GOOGLE DRIVE:\n{authUrl}\n=======================================================\n");

            return await _inner.ReceiveCodeAsync(url, taskCancellationToken);
        }
    }
}
