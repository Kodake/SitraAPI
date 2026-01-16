using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Finances
{
    public sealed class Expense : ITenant
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
