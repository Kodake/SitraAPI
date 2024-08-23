namespace BackEnd.DTOs.Warehouses
{
    public class CreateStockDTO
    {
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
