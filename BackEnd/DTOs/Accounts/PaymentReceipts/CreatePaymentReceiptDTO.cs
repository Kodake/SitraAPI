using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts.PaymentReceipts
{
    public class CreatePaymentReceiptDTO
    {
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
