using SITRA.Core.Entities.Warehouses;

namespace SITRA.Core.DTOs.Reporting
{
    public class StockReportDTO
    {
        public StockReportDTO(List<Stock> stocks, List<long> warehouseIds)
        {
            Stocks = stocks;
            WarehouseIds = warehouseIds;
        }

        public List<Stock> Stocks { get; }
        public List<long> WarehouseIds { get; }
    }
}
