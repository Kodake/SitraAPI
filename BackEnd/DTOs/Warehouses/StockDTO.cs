using BackEnd.Entities.Products;
using BackEnd.Entities.Warehouses;

namespace BackEnd.DTOs.Warehouses
{
    public class StockDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ProductDTO? Product { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseDTO? Warehouse { get; set; }
        public double Qty { get; set; }
        public double? MinQty { get; set; }
        public double? MaxQty { get; set; }
    }
}
