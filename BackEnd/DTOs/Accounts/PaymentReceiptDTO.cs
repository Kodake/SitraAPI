using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts
{
    public class PaymentReceiptDTO
    {
        public long Id { get; set; }
        public long ReceivableId { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public ReceivableDTO? Receivable { get; set; }

    }
}
