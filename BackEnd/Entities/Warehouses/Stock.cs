using BackEnd.Entities.Products;

namespace BackEnd.Entities.Warehouses
{
    public sealed class Stock
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
