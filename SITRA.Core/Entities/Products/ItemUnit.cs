using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class ItemUnit : ITenant, IAuditableEntity
    {
        public long ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public long UnitId { get; set; }
        public virtual Unit Unit { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
