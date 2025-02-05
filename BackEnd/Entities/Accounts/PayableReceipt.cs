using BackEnd.Enums;

namespace BackEnd.Entities.Accounts
{
    public sealed class PayableReceipt
    {
        public long Id { get; set; }
        public long PayableId { get; set; }
        public Payable Payable { get; set; } = null!;
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
