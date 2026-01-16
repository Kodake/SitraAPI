using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PayableReceipts
{
    public class CreatePayableReceiptDTO
    {
        public double Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
