using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;

namespace SITRA.Core.Entities.Invoices
{
    public class SpotPayment : ITenant
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public long? ContactId { get; set; }
        public virtual Contact? Buyer { get; set; } = null!;
        public double AmountReturned { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
