using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Accounts
{
    public sealed class PayableReceipt
    {
        public long Id { get; set; }
        public long PayableId { get; set; }
        public Payable Payable { get; set; } = null!;
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
