using BackEnd.Enums;

namespace BackEnd.Entities.Accounts
{
    public sealed class PaymentReceipt
    {
        public long Id { get; set; }
        public long ReceivableId { get; set; }
        public Receivable Receivable { get; set; } = null!;
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
