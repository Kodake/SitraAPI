namespace SITRA.Core.Models.Dgii
{
    public class DgiiPdfMetadata
    {
        public string? SecurityCode { get; set; }
        public string? SignatureValue { get; set; }
        public string? FechaFirma { get; set; }
        public string? EmisorRnc { get; set; }
        public string? RazonSocialEmisor { get; set; }
        public string? CompradorRnc { get; set; }
        public string? RazonSocialComprador { get; set; }
        public string? Encf { get; set; }
        public decimal MontoTotal { get; set; }
        public string? FechaEmision { get; set; }
        public string? TipoEcf { get; set; }
    }
}
