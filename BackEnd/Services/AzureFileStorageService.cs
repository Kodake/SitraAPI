using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BackEnd.Services
{
    public class AzureFileService : IFileStorageService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public AzureFileService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("AzureStorage")!;
        }

        public async Task<string> SaveAsync(string container, IFormFile file)
        {
            var containerClient = new BlobContainerClient(_connectionString, container);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var blobClient = containerClient.GetBlobClient(fileName);
            var headers = new BlobHttpHeaders { ContentType = file.ContentType };

            await blobClient.UploadAsync(file.OpenReadStream(), headers);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteAsync(string? url, string container)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var containerClient = new BlobContainerClient(_connectionString, container);
            await containerClient.CreateIfNotExistsAsync();

            var fileName = Path.GetFileName(url);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
