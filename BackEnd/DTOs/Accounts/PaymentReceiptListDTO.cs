using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts
{
    public class PaymentReceiptListDTO
    {
        public long Id { get; set; }
        public ReceivableDTO? Receivable { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
