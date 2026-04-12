using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SITRA.Core.Interfaces;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class DgiiApiService : IDgiiApiService
    {
        private readonly HttpClient _httpClient;

        // DGII Endpoints para Pruebas (v1 OAS3)
        private const string SemillaUrl = "https://ecf.dgii.gov.do/CerteCF/Autenticacion/api/Autenticacion/Semilla";
        private const string AutenticacionUrl = "https://ecf.dgii.gov.do/CerteCF/Autenticacion/api/Autenticacion/ValidarSemilla";
        private const string RecepcionUrl = "https://ecf.dgii.gov.do/CerteCF/Recepcion/api/FacturasElectronicas";
        private const string ConsultaUrl = "https://ecf.dgii.gov.do/CerteCF/ConsultaResultado/api/Consultas/Estado";

        public DgiiApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetSemillaAsync()
        {
            var response = await _httpClient.GetAsync(SemillaUrl);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            
            // Si DGII devuelve la semilla como una cadena JSON literal (envuelta en comillas), la limpiamos
            if (content.StartsWith("\"") && content.EndsWith("\""))
            {
                content = content.Substring(1, content.Length - 2);
                content = content.Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\\r", "\r");
            }
            
            return content;
        }

        public async Task<string> GetTokenAsync(string seedXmlSigned)
        {
            using var formData = new MultipartFormDataContent();
            
            // Usamos un codificador puro de UTF-8 sin Byte Order Mark (BOM) para evitar caracteres fantasma
            byte[] xmlBytes = new UTF8Encoding(false).GetBytes(seedXmlSigned);
            var xmlContent = new ByteArrayContent(xmlBytes);
            xmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            
            formData.Add(xmlContent, "xml", "semilla_firmada.xml");

            var response = await _httpClient.PostAsync(AutenticacionUrl, formData);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"DGII Validation Failed ({(int)response.StatusCode}): {errorDetails}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return ExtractTokenFromJson(responseJson);
        }

        public async Task<string> SendEcfAsync(string ecfXmlSigned, string token)
        {
            using var formData = new MultipartFormDataContent();
            
            // Enviamos el XML codificado en binario puro (UTF-8 sin BOM) igual que con la semilla
            byte[] xmlBytes = new UTF8Encoding(false).GetBytes(ecfXmlSigned);
            var xmlContent = new ByteArrayContent(xmlBytes);
            xmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            
            // DGII requiere que el nombre del archivo en el multipart siga un patrón específico:
            // RNCEmisor + TipoeCF + eNCF + .xml
            string fileName = "factura_electronica.xml";
            try
            {
                var doc = System.Xml.Linq.XDocument.Parse(ecfXmlSigned);
                var ns = doc.Root.Name.Namespace;
                var rnc = doc.Descendants(ns + "RNCEmisor").FirstOrDefault()?.Value;
                var encf = doc.Descendants(ns + "eNCF").FirstOrDefault()?.Value;
                
                if (!string.IsNullOrEmpty(rnc) && !string.IsNullOrEmpty(encf))
                {
                    // Clean RNC of any dashes or spaces (common in Excel)
                    var cleanRnc = System.Text.RegularExpressions.Regex.Replace(rnc, @"[^0-9]", "");
                    fileName = $"{cleanRnc}{encf.Trim()}.xml";
                }
            }
            catch { /* Fallback to default if parsing fails */ }

            formData.Add(xmlContent, "xml", fileName);
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync(RecepcionUrl, formData);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"DGII Reception Failed ({(int)response.StatusCode}): {errorDetails}");
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CheckStatusAsync(string trackId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var requestUrl = $"{ConsultaUrl}?trackId={trackId}";
            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractTokenFromJson(string json)
        {
            try
            {
                using var document = System.Text.Json.JsonDocument.Parse(json);
                if (document.RootElement.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }
            catch { }
            
            return string.Empty;
        }
    }
}
