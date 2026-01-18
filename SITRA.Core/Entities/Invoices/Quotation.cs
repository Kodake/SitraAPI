using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Core.Entities.Invoices
{
    public class Quotation : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public long? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; } = null!;
        public long? ContactId { get; set; }
        public virtual Contact? Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public virtual List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
