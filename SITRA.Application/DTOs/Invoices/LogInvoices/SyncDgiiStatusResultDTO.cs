using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.LogInvoices
{
    public class SyncDgiiStatusResultDTO
    {
        public Guid InvoiceId { get; set; }
        public string? FiscalSupportNumber { get; set; }
        public string? PreviousDgiiStatus { get; set; }
        public string? NewDgiiStatus { get; set; }
        public string? DgiiResponse { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public string? Message { get; set; }
    }
}
