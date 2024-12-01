using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts
{
    public class CreatePaymentReceiptDTO
    {
        public long Id { get; set; }
        public long ReceivableId { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
