using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Accounts
{
    public class Payable : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public Guid? ContactId { get; set; }
        public virtual Contact? Supplier { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<PayableReceipt> PayableReceipts { get; set; } = [];
    }
}
