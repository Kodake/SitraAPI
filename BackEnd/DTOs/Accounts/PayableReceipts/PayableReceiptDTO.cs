using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts.PayableReceipts
{
    public class PayableReceiptDTO
    {
        public long Id { get; set; }
        public long PayableId { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public PayableDTO? Payable { get; set; }
    }
}
