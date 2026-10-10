using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.Services.Storage
{
    public class DgiiStorageService : IDgiiStorageService
    {
        private readonly DgiiSettings _settings;
        private readonly string _connectionString;
        private BlobContainerClient? _containerClient;
        private readonly ILogger<DgiiStorageService> _logger;

        // Root blob prefix for schemas — shared across ALL environments
        private const string SchemasBlobPrefix = "Schemas";

        public DgiiStorageService(
            IOptions<DgiiSettings> options,
            IConfiguration configuration,
            ILogger<DgiiStorageService> logger)
        {
            _settings = options.Value;
            _connectionString = configuration.GetConnectionString("AzureStorage") ?? string.Empty;
            _logger = logger;
        }

        // Ensures the container exists and returns the client (lazy, idempotent)
        private async Task<BlobContainerClient> GetContainerAsync()
        {
            if (_containerClient is null)
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new InvalidOperationException("AzureStorage connection string is not configured.");
                }
                _containerClient = new BlobContainerClient(_connectionString, _settings.AzureContainerName);
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            }
            return _containerClient;
        }

        private async Task<BlobClient> GetBlobClientAsync(string rawPath)
        {
            var container = await GetContainerAsync();
            string blobPath = $"{_settings.Environment}/{rawPath}".Replace("\\", "/");
            return container.GetBlobClient(blobPath);
        }

        public async Task<byte[]> ReadCertificateAsync()
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync(_settings.CertificatePath);
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                return ms.ToArray();
            }
            else
            {
                return await File.ReadAllBytesAsync(ResolvePath(_settings.CertificatePath));
            }
        }

        public async Task<byte[]> ReadCertificateFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(SITRA.Core.Helpers.Utilities.CertificateUrlEmptyMessage, nameof(url));

            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var container = await GetContainerAsync();
                Uri uri = new Uri(url);
                string containerPrefix = $"/{_settings.AzureContainerName}/";
                string blobName = uri.AbsolutePath.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase)
                    ? uri.AbsolutePath.Substring(containerPrefix.Length)
                    : uri.AbsolutePath.TrimStart('/');
                var blobClient = container.GetBlobClient(blobName);
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                return ms.ToArray();
            }
            else
            {
                return await File.ReadAllBytesAsync(ResolvePath(url));
            }
        }

        public async Task<Stream> ReadFileStreamAsync(string relativePath)
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync(relativePath);
                var response = await blobClient.DownloadStreamingAsync();
                return response.Value.Content;
            }
            else
            {
                return File.OpenRead(ResolvePath(relativePath));
            }
        }

        public async Task<string?> ReadDocumentXmlAsync(string rnc, string docType, string fileName)
        {
            if (_settings.UseAzureStorage)
            {
                try
                {
                    var blobClient = await GetBlobClientAsync($"{_settings.StoragePath}/{rnc}/{docType}/{fileName}");
                    if (!await blobClient.ExistsAsync()) return null;

                    var response = await blobClient.DownloadContentAsync();
                    return response.Value.Content.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read document XML from Azure for {Rnc}/{DocType}/{FileName}", rnc, docType, fileName);
                    return null;
                }
            }
            else
            {
                string filePath = Path.Combine(ResolvePath(_settings.StoragePath), rnc, docType, fileName);
                if (!File.Exists(filePath)) return null;
                return await File.ReadAllTextAsync(filePath);
            }
        }

        public async Task<string?> ReadDocumentXmlFromUrlAsync(string urlOrPath)
        {
            if (string.IsNullOrEmpty(urlOrPath)) return null;

            try
            {
                if (urlOrPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    urlOrPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    // 1. Try Azure BlobClient if configured
                    if (_settings.UseAzureStorage && !string.IsNullOrWhiteSpace(_connectionString))
                    {
                        try
                        {
                            var container = await GetContainerAsync();
                            Uri uri = new Uri(urlOrPath);
                            string containerPrefix = $"/{_settings.AzureContainerName}/";
                            string blobName = uri.AbsolutePath.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase)
                                ? uri.AbsolutePath.Substring(containerPrefix.Length)
                                : uri.AbsolutePath.TrimStart('/');
                            var blobClient = container.GetBlobClient(blobName);

                            if (await blobClient.ExistsAsync())
                            {
                                var response = await blobClient.DownloadContentAsync();
                                return response.Value.Content.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to read blob via Azure SDK for {Url}, attempting HTTP fallback.", urlOrPath);
                        }
                    }

                    // 2. Direct HTTP GET fallback (for accessible/public blob URLs)
                    try
                    {
                        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                        var res = await httpClient.GetAsync(urlOrPath);
                        if (res.IsSuccessStatusCode)
                        {
                            return await res.Content.ReadAsStringAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch XML via HTTP GET from {Url}", urlOrPath);
                    }

                    return null;
                }
                else
                {
                    if (!File.Exists(urlOrPath)) return null;
                    return await File.ReadAllTextAsync(urlOrPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error reading document XML from {UrlOrPath}", urlOrPath);
                return null;
            }
        }

        public async Task<string> SaveDocumentXmlAsync(string xml, string rnc, string docType, string fileName)
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync($"{_settings.StoragePath}/{rnc}/{docType}/{fileName}");
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                await blobClient.UploadAsync(ms, overwrite: true);
                return blobClient.Uri.ToString();
            }
            else
            {
                string saveDir = Path.Combine(ResolvePath(_settings.StoragePath), rnc, docType);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                string filePath = Path.Combine(saveDir, fileName);
                await File.WriteAllTextAsync(filePath, xml, new UTF8Encoding(false));
                return filePath;
            }
        }

        public async Task<string[]> GetSchemaFilesAsync()
        {
            if (_settings.UseAzureStorage)
            {
                var container = await GetContainerAsync();
                // Schemas live at the ROOT of the container under 'Schemas/' — shared across all environments
                string prefix = $"{SchemasBlobPrefix}/";

                var files = new List<string>();
                await foreach (var blobItem in container.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, default))
                {
                    if (blobItem.Name.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(blobItem.Name);
                    }
                }
                return files.ToArray();
            }
            else
            {
                string schemasPath = ResolvePath(_settings.SchemasPath);
                if (!Directory.Exists(schemasPath)) return Array.Empty<string>();
                return Directory.GetFiles(schemasPath, "*.xsd");
            }
        }

        public async Task<Stream> ReadSchemaStreamAsync(string schemaFileName)
        {
            if (_settings.UseAzureStorage)
            {
                var container = await GetContainerAsync();
                // e.g. "Schemas/e-CF 31 v.1.0.xsd"
                string blobName = $"{SchemasBlobPrefix}/{schemaFileName}";
                var blobClient = container.GetBlobClient(blobName);
                var response = await blobClient.DownloadStreamingAsync();
                return response.Value.Content;
            }
            else
            {
                string filePath = Path.Combine(ResolvePath(_settings.SchemasPath), schemaFileName);
                return File.OpenRead(filePath);
            }
        }

        /// <summary>
        /// Uploads each .xsd file from the local Resources/Schemas directory to the root
        /// 'Schemas/' folder in dgii-assets blob storage, skipping any that already exist.
        /// This is a no-op when UseAzureStorage is false.
        /// </summary>
        public async Task EnsureSchemasUploadedAsync()
        {
            if (!_settings.UseAzureStorage)
            {
                _logger.LogDebug("EnsureSchemasUploadedAsync: Azure Storage is disabled, skipping upload.");
                return;
            }

            string localSchemasDir = ResolvePath(_settings.SchemasPath);
            if (!Directory.Exists(localSchemasDir))
            {
                _logger.LogWarning("EnsureSchemasUploadedAsync: Local schemas directory not found at '{Path}'. Skipping upload.", localSchemasDir);
                return;
            }

            var container = await GetContainerAsync();
            var localFiles = Directory.GetFiles(localSchemasDir, "*.xsd");

            foreach (var localFile in localFiles)
            {
                string fileName = Path.GetFileName(localFile);
                string blobName = $"{SchemasBlobPrefix}/{fileName}";
                var blobClient = container.GetBlobClient(blobName);

                if (await blobClient.ExistsAsync())
                {
                    _logger.LogDebug("EnsureSchemasUploadedAsync: Schema already exists in blob — skipping '{BlobName}'.", blobName);
                    continue;
                }

                _logger.LogInformation("EnsureSchemasUploadedAsync: Uploading schema '{FileName}' to blob '{BlobName}'.", fileName, blobName);
                using var fileStream = File.OpenRead(localFile);
                await blobClient.UploadAsync(fileStream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = "application/xml" }
                });
                _logger.LogInformation("EnsureSchemasUploadedAsync: Uploaded '{BlobName}' successfully.", blobName);
            }
        }

        public async Task<string> SaveCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string rnc)
        {
            string fileName = file.FileName;
            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync($"Certificates/{rnc}/{fileName}");
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }
            else
            {
                string saveDir = Path.Combine(ResolvePath("Resources/Certificates"), rnc);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                string filePath = Path.Combine(saveDir, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                return filePath;
            }
        }

        public async Task<string> SaveCompanyLogoAsync(Microsoft.AspNetCore.Http.IFormFile file, string rnc)
        {
            var extension = Path.GetExtension(file.FileName); // e.g., .png or .jpeg
            string fileName = $"{rnc}-logo{extension}";
            
            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync($"Logos/Logo/{rnc}/{fileName}");
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }
            else
            {
                string saveDir = Path.Combine(ResolvePath("Resources/Logos/Logo"), rnc);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                string filePath = Path.Combine(saveDir, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                return filePath;
            }
        }

        public async Task<string> SaveItemPictureAsync(Microsoft.AspNetCore.Http.IFormFile file, string rnc, Guid? itemId = null)
        {
            const long maxPictureSizeBytes = 300 * 1024; // 300 KB
            if (file.Length > maxPictureSizeBytes)
            {
                throw new InvalidOperationException($"La imagen supera el tamaño máximo permitido de 300 KB ({file.Length} bytes).");
            }

            var extension = Path.GetExtension(file.FileName);
            string fileId = itemId.HasValue ? $"{itemId.Value}_{Guid.NewGuid():N}" : Guid.NewGuid().ToString("N");
            string fileName = $"{fileId}{extension}";

            if (_settings.UseAzureStorage)
            {
                var blobClient = await GetBlobClientAsync($"Items/{rnc}/{fileName}");
                using var stream = file.OpenReadStream();
                var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders
                {
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType
                };
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions { HttpHeaders = headers });
                return blobClient.Uri.ToString();
            }
            else
            {
                string saveDir = Path.Combine(ResolvePath("Resources/Items"), rnc);
                if (!Directory.Exists(saveDir)) Directory.CreateDirectory(saveDir);

                string filePath = Path.Combine(saveDir, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                return filePath;
            }
        }

        public async Task DeleteItemPictureAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                if (_settings.UseAzureStorage)
                {
                    if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        var container = await GetContainerAsync();
                        Uri uri = new Uri(url);
                        string containerPrefix = $"/{_settings.AzureContainerName}/";
                        string blobName = uri.AbsolutePath.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase)
                            ? uri.AbsolutePath.Substring(containerPrefix.Length)
                            : uri.AbsolutePath.TrimStart('/');
                        var blobClient = container.GetBlobClient(blobName);
                        await blobClient.DeleteIfExistsAsync();
                    }
                }
                else
                {
                    if (File.Exists(url))
                    {
                        File.Delete(url);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete item picture: {Url}", url);
            }
        }

        private static string ResolvePath(string path) =>
            Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
    }
}
