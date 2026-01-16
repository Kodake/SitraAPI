using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

namespace SITRA.Core.DTOs.Invoices.Quotations
{
    public class QuotationDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual SaleInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = new List<SaleInvoiceItemDTO>();
    }
}
