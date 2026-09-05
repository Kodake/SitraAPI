using System.Threading.Tasks;

namespace SITRA.Core.Interfaces
{
    public interface IDgiiApiService
    {
        Task<string> GetSemillaAsync();
        Task<string> GetTokenAsync(string seedXmlSigned);
        Task<string> SendEcfAsync(string ecfXmlSigned, string token);
        Task<string> SendRfceAsync(string ecfXmlSigned, string token);
        Task<string> CheckStatusAsync(string trackId, string token);
        Task<string> SendAprobacionComercialAsync(string ecfXmlSigned, string token);
    }
}
