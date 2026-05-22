using System.IO;
using System.Threading.Tasks;

namespace SITRA.Core.Interfaces.Services
{
    public interface IDgiiStorageService
    {
        Task<byte[]> ReadCertificateAsync();
        Task<byte[]> ReadCertificateFromUrlAsync(string url);
        Task<Stream> ReadFileStreamAsync(string relativePath);
        Task<string?> ReadDocumentXmlAsync(string rnc, string docType, string fileName);
        Task<string?> ReadDocumentXmlFromUrlAsync(string urlOrPath);
        Task<string> SaveDocumentXmlAsync(string xml, string rnc, string docType, string fileName);
        Task<string[]> GetSchemaFilesAsync();
        Task<string> SaveCertificateAsync(Microsoft.AspNetCore.Http.IFormFile file, string rnc);
    }
}
