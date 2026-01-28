using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Core.Entities.Invoices
{
    public class SpotPayment : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public Guid? ContactId { get; set; }
        public virtual Contact? Buyer { get; set; } = null!;
        public double AmountReturned { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
