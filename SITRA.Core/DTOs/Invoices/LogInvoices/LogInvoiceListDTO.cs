namespace SITRA.Core.DTOs.Invoices.LogInvoices
{
    public class LogInvoiceListDTO
    {
        public Guid Id { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public string DgiiStatus { get; set; } = string.Empty;
        public string DgiiResponse { get; set; } = string.Empty;
        public string XmlUrl { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
