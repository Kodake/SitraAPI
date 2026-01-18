using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PaymentReceipts
{
    public class PaymentReceiptListDTO
    {
        public long Id { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
