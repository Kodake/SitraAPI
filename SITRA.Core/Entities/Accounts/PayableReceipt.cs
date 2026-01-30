using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Accounts
{
    public sealed class PayableReceipt : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid PayableId { get; set; }
        public Payable Payable { get; set; } = null!;
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
