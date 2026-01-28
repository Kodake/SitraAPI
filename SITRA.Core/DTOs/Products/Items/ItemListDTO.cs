using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Enums;
using System.Text.Json.Serialization;

namespace SITRA.Core.DTOs.Products.Items;
public class ItemListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; } = string.Empty;
    public string? Barcode { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public Guid PricingId { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public double TotalPrice { get; set; }
    public double Qty { get; set; }
    public bool IsActive { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<TaxDTO> Taxes { get; set; } = [];
    [JsonIgnore]
    public List<PricingItemDTO> PricingItems { get; set; } = [];
    [JsonIgnore]
    public List<StockDTO>? Stocks { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
