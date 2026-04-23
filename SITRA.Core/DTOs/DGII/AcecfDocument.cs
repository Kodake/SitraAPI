using System.Xml.Serialization;

namespace SITRA.Core.DTOs.DGII
{
    [XmlRoot(ElementName = "ACECF")]
    public class AcecfDocument
    {
        [XmlElement(ElementName = "DetalleAprobacionComercial")]
        public DetalleAprobacionComercial Detalle { get; set; } = new();
    }

    public class DetalleAprobacionComercial
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; } = "1.0";

        [XmlElement(ElementName = "RNCEmisor")]
        public string RNCEmisor { get; set; } = string.Empty;

        [XmlElement(ElementName = "eNCF")]
        public string ENCF { get; set; } = string.Empty;

        [XmlElement(ElementName = "FechaEmision")]
        public string FechaEmision { get; set; } = string.Empty;

        [XmlElement(ElementName = "MontoTotal")]
        public string MontoTotal { get; set; } = string.Empty;

        [XmlElement(ElementName = "RNCComprador")]
        public string RNCComprador { get; set; } = string.Empty;

        [XmlElement(ElementName = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [XmlElement(ElementName = "DetalleMotivoRechazo")]
        public string? DetalleMotivoRechazo { get; set; }

        [XmlElement(ElementName = "FechaHoraAprobacionComercial")]
        public string FechaHoraAprobacionComercial { get; set; } = string.Empty;

        public bool ShouldSerializeDetalleMotivoRechazo() => !string.IsNullOrEmpty(DetalleMotivoRechazo);
    }
}
