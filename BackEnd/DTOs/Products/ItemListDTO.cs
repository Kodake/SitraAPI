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
    public int PricingId { get; set; }
    public double InitialCost
    {
        get
        {
            return PricingItems?.FirstOrDefault(x => x.PricingId == PricingId)?.InitialCost ?? 0;
        }
    }
    public double BasePrice
    {
        get
        {
            return PricingItems?.FirstOrDefault(x => x.PricingId == PricingId)?.BasePrice ?? 0;
        }
    }
    public double Percentage
    {
        get
        {
            return PricingItems?.FirstOrDefault(x => x.PricingId == PricingId)?.Percentage ?? 0;
        }
    }
    public double TotalPrice
    {
        get
        {
            double totalTaxes = Taxes.Sum(pt => pt.Amount);
            return BasePrice + (BasePrice * totalTaxes / 100);
        }
    }
    public double Qty { get; set; }
    public bool IsActive { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<TaxDTO> Taxes { get; set; } = [];
    [JsonIgnore]
    public List<PricingItemDTO> PricingItems { get; set; } = [];
    [JsonIgnore]
    public List<StockDTO>? Stocks { get; set; } = [];
}