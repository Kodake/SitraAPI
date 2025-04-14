using BackEnd.Entities.Contacts;

namespace BackEnd.Entities.Invoices
{
    public class Quotation
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual Contact Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public virtual List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
