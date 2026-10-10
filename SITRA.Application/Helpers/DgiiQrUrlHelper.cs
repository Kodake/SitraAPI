using System.Text.RegularExpressions;
using System.Web;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Helpers
{
    public static class DgiiQrUrlHelper
    {
        public static string BuildDgiiQrUrl(DgiiPdfMetadata data, string? customUrl = null, string? customFcUrl = null)
        {
            decimal total = data.MontoTotal;
            string encf = (data.Encf ?? "").Trim().ToUpper();
            bool isTipo32 = data.TipoEcf == "32" || encf.StartsWith("E32");
            bool isTipo43 = data.TipoEcf == "43" || encf.StartsWith("E43");
            string normalizedComprador = NormalizeQrDigits(data.CompradorRnc);
            string normalizedEmisor = NormalizeQrDigits(data.EmisorRnc);

            string resolvedFcUrl = string.IsNullOrEmpty(customFcUrl) ? "https://fc.dgii.gov.do/CerteCF/ConsultaTimbreFC" : customFcUrl;
            string resolvedUrl = string.IsNullOrEmpty(customUrl) ? "https://ecf.dgii.gov.do/CerteCF/ConsultaTimbre" : customUrl;

            string baseUrl = isTipo32 && total <= 250000
                ? resolvedFcUrl
                : resolvedUrl;

            var queryParts = new List<string>();
            queryParts.Add($"rncemisor={normalizedEmisor}");
            
            if (!isTipo43 || !string.IsNullOrEmpty(normalizedComprador))
            {
                queryParts.Add($"rnccomprador={normalizedComprador}");
            }
            
            queryParts.Add($"encf={encf}");
            queryParts.Add($"fechaemision={NormalizeQrDate(data.FechaEmision)}");
            queryParts.Add($"montototal={total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
            
            string fechaFirma = NormalizeQrDateTime(data.FechaFirma);
            string fechaFirmaEncoded = fechaFirma.Replace(" ", "%20");
            queryParts.Add($"fechafirma={fechaFirmaEncoded}");
            
            queryParts.Add($"codigoseguridad={(data.SecurityCode ?? "").Trim()}");

            var builder = new UriBuilder(baseUrl);
            builder.Query = string.Join("&", queryParts);
            return builder.ToString();
        }

        private static string NormalizeQrDigits(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Regex.Replace(value, @"[^\d]", "");
        }

        private static string NormalizeQrDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            string text = value.Trim();
            
            var match = Regex.Match(text, @"^(\d{2})-(\d{2})-(\d{4})$");
            if (match.Success) return text;

            var compactMatch = Regex.Match(text, @"^(\d{2})(\d{2})(\d{4})$");
            if (compactMatch.Success)
            {
                return $"{compactMatch.Groups[1].Value}-{compactMatch.Groups[2].Value}-{compactMatch.Groups[3].Value}";
            }

            return text;
        }

        private static string NormalizeQrDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            string text = value.Trim();
            
            var match = Regex.Match(text, @"^(\d{2})-(\d{2})-(\d{4})\s+(\d{2}):(\d{2}):(\d{2})$");
            if (match.Success) return text;

            var compactMatch = Regex.Match(text, @"^(\d{2})(\d{2})(\d{4})\s+(\d{2}):(\d{2}):(\d{2})$");
            if (compactMatch.Success)
            {
                return $"{compactMatch.Groups[1].Value}-{compactMatch.Groups[2].Value}-{compactMatch.Groups[3].Value} {compactMatch.Groups[4].Value}:{compactMatch.Groups[5].Value}:{compactMatch.Groups[6].Value}";
            }

            return text;
        }
    }
}
