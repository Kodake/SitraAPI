using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.DTOs.Invoices.SaleInvoices;

namespace BackEnd.DTOs.Invoices.Quotations
{
    public class QuotationDTO
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public virtual SaleInvoiceDTO SaleInvoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItemDTO>();
    }
}
