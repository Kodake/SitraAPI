using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class ComboItem : ITenant, IAuditableEntity
    {
        public Guid ParentItemId { get; set; }
        public Item ParentItem { get; set; } = null!;
        public Guid ChildItemId { get; set; }
        public Item ChildItem { get; set; } = null!;
        public double Qty { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
