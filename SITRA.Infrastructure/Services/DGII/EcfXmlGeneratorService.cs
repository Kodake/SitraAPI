using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SITRA.Core.Interfaces;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class EcfXmlGeneratorService : IEcfXmlGenerator
    {
        public string GenerateEcfXml(EcfDocument document)
        {
            return SerializeToXml(document);
        }

        public string GenerateRfceXml(RfceDocument document)
        {
            return SerializeToXml(document);
        }

        private static string SerializeToXml<T>(T document)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false), // Disable BOM
                Indent = true,
                OmitXmlDeclaration = false
            };

            var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var memoryStream = new MemoryStream())
            using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
            {
                serializer.Serialize(xmlWriter, document, emptyNamespaces);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }
}
