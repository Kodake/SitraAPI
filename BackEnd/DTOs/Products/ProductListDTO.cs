using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Warehouses;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products;
public class ProductListDTO
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
}