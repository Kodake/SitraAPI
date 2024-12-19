namespace BackEnd.DTOs.Warehouses.Stocks
{
    public class CreateStockDTO
    {
        public long ItemId { get; set; }
        public long WarehouseId { get; set; }
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
