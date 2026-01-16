using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PayableReceipts
{
    public class PayableReceiptListDTO
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
    }
}
