using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Infrastructure.Services.DGII
{
    public class EcfXmlGeneratorService
    {
        public string GenerateEcfXml(EcfDocument document)
        {
            var serializer = new XmlSerializer(typeof(EcfDocument));

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
