namespace BackEnd.DTOs.Warehouses
{
    public class StockDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ProductDTO Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse { get; set; } = null!;
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
