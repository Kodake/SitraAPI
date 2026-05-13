using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.Services
{
    public class DgiiStorageService : IDgiiStorageService
    {
        private readonly DgiiSettings _settings;
        private readonly string _connectionString;

        public DgiiStorageService(IOptions<DgiiSettings> options, IConfiguration configuration)
        {
            _settings = options.Value;
            _connectionString = configuration.GetConnectionString("AzureStorage") ?? string.Empty;
        }

        public async Task<byte[]> ReadCertificateAsync()
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = GetBlobClient(_settings.CertificatePath);
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                return ms.ToArray();
            }
            else
            {
                return await File.ReadAllBytesAsync(_settings.CertificatePath);
            }
        }

        public async Task<Stream> ReadFileStreamAsync(string relativePath)
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = GetBlobClient(relativePath);
                var response = await blobClient.DownloadStreamingAsync();
                return response.Value.Content;
            }
            else
            {
                return File.OpenRead(relativePath);
            }
        }

        public async Task<string?> ReadDocumentXmlAsync(string rnc, string docType, string fileName)
        {
            if (_settings.UseAzureStorage)
            {
                var blobClient = GetBlobClient($"{_settings.StoragePath}/{rnc}/{docType}/{fileName}");
                if (!await blobClient.ExistsAsync()) return null;

                var response = await blobClient.DownloadContentAsync();
                return response.Value.Content.ToString();
            }
            else
            {
                string filePath = Path.Combine(_settings.StoragePath, rnc, docType, fileName);
                if (!File.Exists(filePath)) return null;
                return await File.ReadAllTextAsync(filePath);
            }
        }

        public async Task<string?> ReadDocumentXmlFromUrlAsync(string urlOrPath)
        {
            if (string.IsNullOrEmpty(urlOrPath)) return null;

            if (_settings.UseAzureStorage && urlOrPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                // urlOrPath is the absolute blob URI, we need to extract the blob name
                // e.g. https://account.blob.core.windows.net/container/qa/GeneratedData/123/32/file.xml
                Uri uri = new Uri(urlOrPath);
                string blobName = uri.AbsolutePath.Substring($"/{_settings.AzureContainerName}/".Length);
                var blobClient = GetBlobClient(blobName.Substring(_settings.Environment.Length + 1)); // adjust prefix if needed
                
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
                var blobClient = GetBlobClient($"{_settings.StoragePath}/{rnc}/{docType}/{fileName}");
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                await blobClient.UploadAsync(ms, overwrite: true);
                return blobClient.Uri.ToString();
            }
            else
            {
                string saveDir = Path.Combine(_settings.StoragePath, rnc, docType);
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
                // In Azure Blob Storage, we would list blobs in the SchemasPath directory
                var containerClient = new BlobContainerClient(_connectionString, _settings.AzureContainerName);
                string prefix = $"{_settings.Environment}/{_settings.SchemasPath}/".TrimStart('/');
                
                var files = new System.Collections.Generic.List<string>();
                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
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
                if (!Directory.Exists(_settings.SchemasPath)) return Array.Empty<string>();
                return Directory.GetFiles(_settings.SchemasPath, "*.xsd");
            }
        }

        private BlobClient GetBlobClient(string rawPath)
        {
            var containerClient = new BlobContainerClient(_connectionString, _settings.AzureContainerName);
            // Prepend the environment folder to separate QA, Prod, etc.
            string blobPath = $"{_settings.Environment}/{rawPath}".Replace("\\", "/");
            return containerClient.GetBlobClient(blobPath);
        }
    }
}
