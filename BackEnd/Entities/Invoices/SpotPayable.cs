using BackEnd.Entities.Contacts;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public class SpotPayable
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual Contact Supplier { get; set; } = null!;
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod? PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
