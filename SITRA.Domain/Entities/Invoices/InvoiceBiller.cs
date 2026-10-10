using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Invoices
{
    public class InvoiceBiller : ITenant, IAuditableEntity
    {
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public Guid ContactId { get; set; }
        public virtual Contact Biller { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
