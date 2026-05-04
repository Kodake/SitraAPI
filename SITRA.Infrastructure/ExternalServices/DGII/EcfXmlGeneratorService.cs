using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.ExternalServices.DGII
{
    public class EcfXmlGeneratorService : IEcfXmlGenerator
    {
        public string GenerateEcfXml(EcfDocument document)
        {
            string xml = SerializeToXml(document);
            xml = FormatDecimalElements(xml);       // ← nuevo paso
            xml = RemoveEmptyElements(xml);
            xml = MoveFechaHoraFirmaToEnd(xml);
            return NormalizeXmlDeclaration(xml);
        }

        public string GenerateRfceXml(RfceDocument document)
        {
            string xml = SerializeToXml(document);
            xml = RemoveEmptyElements(xml);
            // Ensure any timestamp element is at the end if present
            xml = MoveFechaHoraFirmaToEnd(xml);
            return NormalizeXmlDeclaration(xml);
        }

        public string GenerateAcecfXml(AcecfDocument document)
        {
            string xml = SerializeToXml(document);
            xml = RemoveEmptyElements(xml);
            return NormalizeXmlDeclaration(xml);
        }

        private static string SerializeToXml<T>(T document)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true
            };

            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");

            // Forzar InvariantCulture para que decimales siempre usen punto
            // y se serialicen con 2 decimales (500.00 en lugar de 500)
            var prevCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.InvariantCulture;

            try
            {
                using var stringWriter = new Utf8StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, settings);
                serializer.Serialize(xmlWriter, document, emptyNamespaces);
                return stringWriter.ToString();
            }
            finally
            {
                // Siempre restaurar la cultura original, incluso si hay excepción
                System.Threading.Thread.CurrentThread.CurrentCulture = prevCulture;
            }
        }

        private static readonly HashSet<string> DecimalElements = new()
        {
            "MontoGravadoTotal", "MontoGravadoI1", "MontoGravadoI2", "MontoGravadoI3",
            "MontoExento", "TotalITBIS", "TotalITBIS1", "TotalITBIS2", "TotalITBIS3",
            "MontoTotal", "MontoItem", "PrecioUnitarioItem", "CantidadItem",
            "MontoPago", "ValorPagar", "SaldoAnterior", "MontoPeriodo",
            "MontoAvancePago", "MontoNoFacturable", "MontoImpuestoAdicional",
            "TotalITBISRetenido", "TotalISRRetencion", "TotalITBISPercepcion", "TotalISRPercepcion"
        };

        private static string FormatDecimalElements(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);

                foreach (var element in doc.Descendants())
                {
                    if (!DecimalElements.Contains(element.Name.LocalName)) continue;
                    if (element.HasElements) continue;

                    var raw = element.Value.Trim();
                    if (string.IsNullOrEmpty(raw)) continue;

                    // Si ya tiene punto decimal con 2+ dígitos, dejarlo
                    if (System.Text.RegularExpressions.Regex.IsMatch(raw, @"^\-?\d+\.\d{2,}$"))
                        continue;

                    // Parsear y reformatear siempre con 2 decimales
                    if (decimal.TryParse(raw,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out decimal value))
                    {
                        element.Value = value.ToString("F2",
                            System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                return doc.ToString();
            }
            catch
            {
                return xml;
            }
        }

        private static string RemoveEmptyElements(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                if (doc.Root == null) return xml;

                RemoveEmptyElementsRecursive(doc.Root);
                return doc.ToString();
            }
            catch
            {
                return xml;
            }
        }

        private static void RemoveEmptyElementsRecursive(XElement element)
        {
            var children = element.Elements().ToList();
            foreach (var child in children)
            {
                RemoveEmptyElementsRecursive(child);
            }

            // Remove if no children and no value (or only whitespace)
            if (!element.HasElements && string.IsNullOrWhiteSpace(element.Value))
            {
                element.Remove();
            }
        }

        private static string MoveFechaHoraFirmaToEnd(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var root = doc.Root;
                if (root == null) return xml;

                var fechaHoraFirma = root.Element("FechaHoraFirma");
                if (fechaHoraFirma != null)
                {
                    fechaHoraFirma.Remove();
                    root.Add(fechaHoraFirma);
                    return doc.ToString();
                }
                return xml;
            }
            catch
            {
                return xml;
            }
        }

        private static string NormalizeXmlDeclaration(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return xml;

            var normalized = xml.TrimStart();

            // Remove any existing XML declarations if they somehow crept in
            if (normalized.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
            {
                int endDecl = normalized.IndexOf("?>");
                if (endDecl != -1)
                {
                    normalized = normalized.Substring(endDecl + 2).TrimStart();
                }
            }

            // DGII often prefers simple declaration without encoding or with specific encoding
            return "<?xml version=\"1.0\"?>" + Environment.NewLine + normalized;
        }

        private sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => new UTF8Encoding(false);
        }
    }
}
