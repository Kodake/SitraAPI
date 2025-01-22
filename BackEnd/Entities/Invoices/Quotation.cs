using BackEnd.Entities.Contacts;

namespace BackEnd.Entities.Invoices
{
    public class Quotation
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public virtual SaleInvoice SaleInvoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual Contact Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public virtual List<SaleInvoiceItem> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItem>();
    }
}
