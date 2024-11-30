using BackEnd.Enums;

namespace BackEnd.Entities.Accounts
{
    public sealed class PaymentReceipt
    {
        public long Id { get; set; }
        public long AccountsReceivableId { get; set; }
        public Receivable AccountsReceivable { get; set; } = null!;
        public double Amount { get; set; }
        public Account Account { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
