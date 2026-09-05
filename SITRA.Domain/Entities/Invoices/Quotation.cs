using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Invoices
{
    public class Quotation : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; } = null!;
        public Guid? ContactId { get; set; }
        public virtual Contact? Buyer { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public virtual List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
