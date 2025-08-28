using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products.Combos;
public class ComboListDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public double TotalPrice { get; set; }
    public bool IsActive { get; set; }
    public List<TaxDTO> Taxes { get; set; } = [];
    [JsonIgnore]
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
    public List<StockDTO>? Stocks { get; set; } = [];
    public List<ComboDTO> ComboItems { get; set; } = [];
}