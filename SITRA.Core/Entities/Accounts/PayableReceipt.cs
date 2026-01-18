using SITRA.Core.Interfaces;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Accounts
{
    public sealed class PayableReceipt : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public long PayableId { get; set; }
        public Payable Payable { get; set; } = null!;
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
