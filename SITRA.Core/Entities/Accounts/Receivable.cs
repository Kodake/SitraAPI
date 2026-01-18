using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Accounts
{
    public class Receivable : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public long? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; } = null!;
        public long? ContactId { get; set; }
        public virtual Contact? Buyer { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<PaymentReceipt> PaymentReceipts { get; set; } = [];
    }
}
