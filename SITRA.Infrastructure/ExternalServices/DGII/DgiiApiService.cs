using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SITRA.Core.Interfaces;
using SITRA.Core.Models.Dgii;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class DgiiApiService : IDgiiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DgiiSettings _settings;

        private string SemillaUrl => string.IsNullOrEmpty(_settings.SemillaUrl) ? $"{_settings.BaseUrl}/CerteCF/Autenticacion/api/Autenticacion/Semilla" : _settings.SemillaUrl;
        private string AutenticacionUrl => string.IsNullOrEmpty(_settings.AutenticacionUrl) ? $"{_settings.BaseUrl}/CerteCF/Autenticacion/api/Autenticacion/ValidarSemilla" : _settings.AutenticacionUrl;
        private string RecepcionUrl => string.IsNullOrEmpty(_settings.RecepcionUrl) ? $"{_settings.BaseUrl}/CerteCF/Recepcion/api/FacturasElectronicas" : _settings.RecepcionUrl;
        private string ConsultaUrl => string.IsNullOrEmpty(_settings.ConsultaUrl) ? $"{_settings.BaseUrl}/CerteCF/ConsultaResultado/api/Consultas/Estado" : _settings.ConsultaUrl;
        private string RecepcionResumenUrl => string.IsNullOrEmpty(_settings.RecepcionResumenUrl) ? "https://fc.dgii.gov.do/CerteCF/RecepcionFC/api/Recepcion/Ecf" : _settings.RecepcionResumenUrl;
        private string AprobacionComercial => string.IsNullOrEmpty(_settings.AprobacionComercialUrl) ? $"{_settings.BaseUrl}/CerteCF/AprobacionComercial/api/AprobacionComercial" : _settings.AprobacionComercialUrl;

        public DgiiApiService(HttpClient httpClient, IOptions<DgiiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
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
            return await SendToDgiiAsync(ecfXmlSigned, token, RecepcionUrl);
        }

        public async Task<string> SendRfceAsync(string ecfXmlSigned, string token)
        {
            return await SendToDgiiAsync(ecfXmlSigned, token, RecepcionResumenUrl);
        }

        public async Task<string> SendAprobacionComercialAsync(string ecfXmlSigned, string token)
        {
            return await SendToDgiiAsync(ecfXmlSigned, token, AprobacionComercial);
        }

        private async Task<string> SendToDgiiAsync(string ecfXmlSigned, string token, string targetUrl)
        {
            using var formData = new MultipartFormDataContent();
            
            // Enviamos el XML codificado en binario puro (UTF-8 sin BOM) igual que con la semilla
            byte[] xmlBytes = new UTF8Encoding(false).GetBytes(ecfXmlSigned);
            var xmlContent = new ByteArrayContent(xmlBytes);
            xmlContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            
            // DGII requiere que el nombre del archivo en el multipart siga un patrn especfico:
            // RNCEmisor + TipoeCF + eNCF + .xml
            string fileName = "factura_electronica.xml";
            try
            {
                var doc = System.Xml.Linq.XDocument.Parse(ecfXmlSigned);
                var ns = doc.Root.Name.Namespace;
                bool isAcecf = doc.Root.Name.LocalName == "ACECF";

                var rnc = isAcecf 
                    ? doc.Descendants(ns + "RNCComprador").FirstOrDefault()?.Value 
                    : doc.Descendants(ns + "RNCEmisor").FirstOrDefault()?.Value;
                    
                var encf = doc.Descendants(ns + "eNCF").FirstOrDefault()?.Value;
                
                if (!string.IsNullOrEmpty(rnc) && !string.IsNullOrEmpty(encf))
                {
                    // Clean RNC of any dashes or spaces (common in Excel)
                    var cleanRnc = System.Text.RegularExpressions.Regex.Replace(rnc, @"[^0-9]", "");
                    fileName = isAcecf 
                        ? $"{cleanRnc}{encf.Trim()}ACECF.xml"
                        : $"{cleanRnc}{encf.Trim()}.xml";
                }
            }
            catch { /* Fallback to default if parsing fails */ }

            formData.Add(xmlContent, "xml", fileName);
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync(targetUrl, formData);
            
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
