using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts.PaymentReceipts
{
    public class PaymentReceiptListDTO
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
    }
}
