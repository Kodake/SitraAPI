using SITRA.Core.Entities.Contacts;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Invoices
{
    public class SpotPayable : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; } = null!;
        public Guid? ContactId { get; set; }
        public virtual Contact? Supplier { get; set; } = null!;
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
