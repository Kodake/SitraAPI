using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PayableReceipts
{
    public class PayableReceiptListDTO
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
