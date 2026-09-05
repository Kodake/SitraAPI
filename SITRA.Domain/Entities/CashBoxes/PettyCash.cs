using SITRA.Core.Entities.Auths;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.CashBoxes
{
    public sealed class PettyCash : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public double OpeningAmount { get; set; }
        public DateTimeOffset OpeningDate { get; set; }
        public double? ClosingAmount { get; set; }
        public double? WithdrawalAmount { get; set; }
        public DateTimeOffset? ClosingDate { get; set; }
        public PettyCashStatus Status { get; set; }
        public string? Comments { get; set; }
        public ICollection<PettyCashMovement> PettyCashMovements { get; set; } = [];
        public Guid TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
