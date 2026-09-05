using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class ItemModel : ITenant, IAuditableEntity
    {
        public Guid ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public Guid ModelId { get; set; }
        public virtual Model Model { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
