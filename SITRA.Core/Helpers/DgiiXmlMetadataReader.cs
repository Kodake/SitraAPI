using System.Xml.Linq;
using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Helpers
{
    public static class DgiiXmlMetadataReader
    {
        public static DgiiPdfMetadata? TryExtractMetadata(string storagePath, string ownerRnc, string fiscalSupportNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fiscalSupportNumber) || fiscalSupportNumber.Length < 3)
                    return null;

                string docType = fiscalSupportNumber.Substring(1, 2); // E31 -> 31, E32 -> 32
                string baseDir = !string.IsNullOrEmpty(storagePath) ? storagePath : string.Empty;
                
                if (string.IsNullOrEmpty(baseDir))
                    return null;

                string filePath = Path.Combine(baseDir, ownerRnc, docType, $"{ownerRnc}{fiscalSupportNumber}.xml");

                if (!File.Exists(filePath))
                {
                    // Try fallback to consolidated RFCE directory as instructed
                    string rfcePath = Path.Combine(baseDir, ownerRnc, "RFCE", $"{ownerRnc}{fiscalSupportNumber}_RFCE.xml");
                    if (File.Exists(rfcePath))
                    {
                        filePath = rfcePath;
                    }
                    else
                    {
                        return null;
                    }
                }

                string xml = File.ReadAllText(filePath);
                var doc = XDocument.Parse(xml);
                
                var rootNodeName = doc.Root?.Name.LocalName;
                bool isRfce = rootNodeName == "RFCE";

                XNamespace ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
                XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";

                var encabezado = doc.Descendants(ns + "Encabezado").FirstOrDefault();
                var idDoc = encabezado?.Element(ns + "IdDoc");
                var emisor = encabezado?.Element(ns + "Emisor");
                var comprador = encabezado?.Element(ns + "Comprador");
                var totales = encabezado?.Element(ns + "Totales");

                string? sigValue = doc.Descendants(ds + "SignatureValue").FirstOrDefault()?.Value;
                if (!string.IsNullOrEmpty(sigValue))
                {
                    sigValue = sigValue.Replace("\n", "").Replace("\r", "").Replace(" ", "");
                }

                string? secCodeXml = encabezado?.Element(ns + "CodigoSeguridadeCF")?.Value 
                                  ?? doc.Root?.Element(ns + "CodigoSeguridadeCF")?.Value;

                string secCode = !string.IsNullOrEmpty(secCodeXml) ? secCodeXml : (sigValue?.Length >= 6 ? sigValue.Substring(0, 6) : "000000");

                string? totalStr = totales?.Element(ns + "MontoTotal")?.Value;
                decimal total = decimal.TryParse(totalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var t) ? t : 0;

                string? gravadoStr = totales?.Element(ns + "MontoGravadoTotal")?.Value;
                decimal gravado = decimal.TryParse(gravadoStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var g) ? g : 0;

                string? itbisStr = totales?.Element(ns + "TotalITBIS")?.Value;
                decimal itbis = decimal.TryParse(itbisStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i) ? i : 0;

                var items = new List<DgiiPdfItemMetadata>();
                var itemNodes = doc.Descendants(ns + "DetallesItems").Elements(ns + "Item");
                foreach (var node in itemNodes)
                {
                    decimal.TryParse(node.Element(ns + "CantidadItem")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var qty);
                    decimal.TryParse(node.Element(ns + "PrecioUnitarioItem")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var unit);
                    decimal.TryParse(node.Element(ns + "MontoItem")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var monto);

                    items.Add(new DgiiPdfItemMetadata
                    {
                        NombreItem = node.Element(ns + "NombreItem")?.Value,
                        CantidadItem = qty,
                        PrecioUnitarioItem = unit,
                        MontoItem = monto
                    });
                }

                return new DgiiPdfMetadata
                {
                    SecurityCode = secCode,
                    SignatureValue = sigValue,
                    FechaFirma = doc.Root?.Element(ns + "FechaHoraFirma")?.Value,
                    EmisorRnc = emisor?.Element(ns + "RNCEmisor")?.Value ?? ownerRnc,
                    RazonSocialEmisor = emisor?.Element(ns + "RazonSocial")?.Value ?? emisor?.Element(ns + "RazonSocialEmisor")?.Value,
                    DireccionEmisor = emisor?.Element(ns + "DireccionEmisor")?.Value,
                    CompradorRnc = comprador?.Element(ns + "RNCComprador")?.Value ?? comprador?.Element(ns + "IdentificadorExtranjero")?.Value,
                    RazonSocialComprador = comprador?.Element(ns + "RazonSocialComprador")?.Value,
                    DireccionComprador = comprador?.Element(ns + "DireccionComprador")?.Value,
                    CorreoComprador = comprador?.Element(ns + "CorreoComprador")?.Value,
                    MunicipioComprador = comprador?.Element(ns + "MunicipioComprador")?.Value,
                    ProvinciaComprador = comprador?.Element(ns + "ProvinciaComprador")?.Value,
                    Encf = idDoc?.Element(ns + "eNCF")?.Value ?? fiscalSupportNumber,
                    MontoTotal = total,
                    MontoGravadoTotal = gravado,
                    TotalItbis = itbis,
                    FechaEmision = emisor?.Element(ns + "FechaEmision")?.Value,
                    TipoEcf = idDoc?.Element(ns + "TipoeCF")?.Value ?? docType,
                    FechaVencimientoSecuencia = idDoc?.Element(ns + "FechaVencimientoSecuencia")?.Value,
                    IsRfce = isRfce,
                    Items = items
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
