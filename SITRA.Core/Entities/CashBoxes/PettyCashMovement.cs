using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;

namespace SITRA.Core.Entities.CashBoxes
{
    public sealed class PettyCashMovement : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid PettyCashId { get; set; }
        public PettyCash PettyCash { get; set; } = null!;
        public Guid? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; } = null!;
        public double Amount { get; set; }
        public MovementType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
