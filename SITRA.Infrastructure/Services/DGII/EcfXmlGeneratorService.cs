using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class EcfXmlGeneratorService : IEcfXmlGenerator
    {
        public string GenerateEcfXml(EcfDocument document)
        {
            string xml = SerializeToXml(document);
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
                Encoding = new UTF8Encoding(false), // Disable BOM
                Indent = true,
                IndentChars = "  ",
                OmitXmlDeclaration = true // We handle the declaration in NormalizeXmlDeclaration
            };

            // Remove default namespaces (xsi and xsd) as they can cause issues with DGII signature
            var emptyNamespaces = new XmlSerializerNamespaces();
            emptyNamespaces.Add("", "");

            using (var stringWriter = new Utf8StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                serializer.Serialize(xmlWriter, document, emptyNamespaces);
                return stringWriter.ToString();
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
