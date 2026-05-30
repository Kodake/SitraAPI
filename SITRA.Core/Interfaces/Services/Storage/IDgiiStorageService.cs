using System.IO;
using System.Threading.Tasks;

namespace SITRA.Core.Interfaces.Services.Storage
{
    public interface IDgiiStorageService
    {
        Task<byte[]> ReadCertificateAsync();
        Task<byte[]> ReadCertificateFromUrlAsync(string url);
        Task<Stream> ReadFileStreamAsync(string relativePath);
        Task<string?> ReadDocumentXmlAsync(string rnc, string docType, string fileName);
        Task<string?> ReadDocumentXmlFromUrlAsync(string urlOrPath);
        Task<string> SaveDocumentXmlAsync(string xml, string rnc, string docType, string fileName);
        /// <summary>Returns the blob names (or local paths) of all XSD schema files.</summary>
        Task<string[]> GetSchemaFilesAsync();
        /// <summary>Opens a read stream for the given schema file name from blob storage (root Schemas/ prefix) or local fallback.</summary>
        Task<Stream> ReadSchemaStreamAsync(string schemaFileName);
        /// <summary>Uploads any locally-embedded XSD schemas to the root 'Schemas/' folder in blob storage if they do not already exist. No-op when Azure Storage is disabled.</summary>
        Task EnsureSchemasUploadedAsync();
        Task<string> SaveCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string rnc);
    }
}
