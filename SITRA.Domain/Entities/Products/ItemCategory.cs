using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class ItemCategory : ITenant, IAuditableEntity
    {
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
