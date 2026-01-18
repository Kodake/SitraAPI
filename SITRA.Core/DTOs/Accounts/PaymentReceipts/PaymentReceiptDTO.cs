using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.PaymentReceipts
{
    public class PaymentReceiptDTO
    {
        public long Id { get; set; }
        public long ReceivableId { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; }
        public ReceivableDTO? Receivable { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
