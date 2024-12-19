using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Products.Units;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products.Items;
public class ItemListDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public long PricingId { get; set; }
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
}