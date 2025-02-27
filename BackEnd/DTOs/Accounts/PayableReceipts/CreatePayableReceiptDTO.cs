using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts.PayableReceipts
{
    public class CreatePayableReceiptDTO
    {
        public double Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
