using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Warehouses;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products;
public class ItemListDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public double? TotalPrice { get; set; }
    public bool IsActive { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<TaxDTO> Taxes { get; set; } = new List<TaxDTO>();
    [JsonIgnore]
    public List<PricingItemDTO> PricingItems { get; set; } = new List<PricingItemDTO>();
    public List<StockDTO>? Stocks { get; set; } = new List<StockDTO>();
}