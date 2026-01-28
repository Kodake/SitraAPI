using SITRA.Core.Interfaces;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.Accounts
{
    public sealed class PaymentReceipt : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid ReceivableId { get; set; }
        public Receivable Receivable { get; set; } = null!;
        public double Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
