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
        
        public string? DireccionEmisor { get; set; }
        public string? DireccionComprador { get; set; }
        public string? CorreoComprador { get; set; }
        public string? MunicipioComprador { get; set; }
        public string? ProvinciaComprador { get; set; }
        public string? FechaVencimientoSecuencia { get; set; }

        public decimal MontoGravadoTotal { get; set; }
        public decimal TotalItbis { get; set; }
        public bool IsRfce { get; set; }

        public List<DgiiPdfItemMetadata> Items { get; set; } = new();
    }

    public class DgiiPdfItemMetadata
    {
        public string? NombreItem { get; set; }
        public decimal CantidadItem { get; set; }
        public decimal PrecioUnitarioItem { get; set; }
        public decimal MontoItem { get; set; }
    }
}
