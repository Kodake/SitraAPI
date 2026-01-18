using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PayableReceipts
{
    public class PayableReceiptDTO
    {
        public long Id { get; set; }
        public long PayableId { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public PayableDTO? Payable { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
