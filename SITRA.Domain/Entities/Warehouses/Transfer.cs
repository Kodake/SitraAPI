using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Warehouses
{
    public class Transfer : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Item? Product { get; set; } = null!;
        public Guid OriginWarehouseId { get; set; }
        public Warehouse OriginWarehouse { get; set; } = null!;
        public Guid DestinationWarehouseId { get; set; }
        public Warehouse DestinationWarehouse { get; set; } = null!;
        public double Qty { get; set; }
        public Guid StockId { get; set; }
        public Stock? Stock { get; set; }
        public string Comment { get; set; } = string.Empty;
        public TransferStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
