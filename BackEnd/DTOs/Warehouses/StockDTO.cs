namespace BackEnd.DTOs.Warehouses
{
    public class StockDTO
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public ProductDTO Product { get; set; } = null!;
        public long WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
