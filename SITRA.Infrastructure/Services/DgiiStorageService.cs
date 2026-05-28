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

namespace SITRA.Infrastructure.Services
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
                Uri uri = new Uri(url);
                string blobName = uri.AbsolutePath.Substring($"/{_settings.AzureContainerName}/".Length);
                // Strip the leading environment prefix already embedded in the URI
                string relativePath = blobName.Substring(_settings.Environment.Length + 1);
                var blobClient = await GetBlobClientAsync(relativePath);
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
                var blobClient = await GetBlobClientAsync($"{_settings.StoragePath}/{rnc}/{docType}/{fileName}");
                if (!await blobClient.ExistsAsync()) return null;

                var response = await blobClient.DownloadContentAsync();
                return response.Value.Content.ToString();
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

            if (_settings.UseAzureStorage && urlOrPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // urlOrPath is the absolute blob URI — extract the blob name after the container segment
                Uri uri = new Uri(urlOrPath);
                string blobName = uri.AbsolutePath.Substring($"/{_settings.AzureContainerName}/".Length);
                // Strip the leading environment prefix (e.g. "QA/") already embedded in the URI
                var blobClient = await GetBlobClientAsync(blobName.Substring(_settings.Environment.Length + 1));

                if (!await blobClient.ExistsAsync()) return null;
                var response = await blobClient.DownloadContentAsync();
                return response.Value.Content.ToString();
            }
            else
            {
                if (!File.Exists(urlOrPath)) return null;
                return await File.ReadAllTextAsync(urlOrPath);
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

        private static string ResolvePath(string path) =>
            Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path);
    }
}
