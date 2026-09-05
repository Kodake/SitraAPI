using System.Xml.Serialization;

namespace SITRA.Core.DTOs.DGII
{
    [XmlRoot(ElementName = "ARECF", Namespace = "")]
    public class ArecfDocumentTest
    {
        [XmlElement(ElementName = "DetalleAcusedeRecibo")]
        public DetalleAcusedeRecibo Detalle { get; set; } = new();
    }

    public class DetalleAcusedeRecibo
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; } = "1.0";

        [XmlElement(ElementName = "RNCEmisor")]
        public string RNCEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "RNCComprador")]
        public string RNCComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "eNCF")]
        public string ENCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "Estado")]
        public int Estado { get; set; }

        [XmlElement(ElementName = "FechaHoraAcuseRecibo")]
        public string FechaHoraAcuseRecibo { get; set; } = string.Empty;
    }
}
