using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Finances
{
    public sealed class Expense : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
