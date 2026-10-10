using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PaymentReceipts
{
    public class CreatePaymentReceiptDTO
    {
        public double Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
