using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Warehouses
{
    public sealed class Stock : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
