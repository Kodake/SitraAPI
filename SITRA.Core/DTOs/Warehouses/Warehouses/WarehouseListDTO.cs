using SITRA.Core.DTOs.Warehouses.Stocks;

namespace SITRA.Core.DTOs.Warehouses.Warehouses
{
    public class WarehouseListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public List<StockDTO> Stocks { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
