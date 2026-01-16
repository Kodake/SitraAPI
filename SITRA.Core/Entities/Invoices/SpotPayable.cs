using SITRA.Core.Entities.Contacts;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Invoices
{
    public class SpotPayable
    {
        public long Id { get; set; }
        public long? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; } = null!;
        public long? ContactId { get; set; }
        public virtual Contact? Supplier { get; set; } = null!;
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod? PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
