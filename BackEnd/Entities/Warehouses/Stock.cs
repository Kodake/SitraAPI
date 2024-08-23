using BackEnd.Entities.Products;

namespace BackEnd.Entities.Warehouses
{
    public sealed class Stock
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
