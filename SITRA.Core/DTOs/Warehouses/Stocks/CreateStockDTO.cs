namespace SITRA.Core.DTOs.Warehouses.Stocks
{
    public class CreateStockDTO
    {
        public Guid ItemId { get; set; }
        public Guid WarehouseId { get; set; }
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
