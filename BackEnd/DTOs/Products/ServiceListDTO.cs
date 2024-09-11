using BackEnd.DTOs.Finances;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products;
public class ServiceListDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public double BasePrice
    {
        get
        {
            return PricingItems?.FirstOrDefault(p => p.IsPrincipal)?.BasePrice ?? 0;
        }
    }
    public double Percentage
    {
        get
        {
            return PricingItems?.FirstOrDefault(p => p.IsPrincipal)?.Percentage ?? 0;
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
    public bool IsActive { get; set; }
    public List<TaxDTO> Taxes { get; set; } = new List<TaxDTO>();
    [JsonIgnore]
    public List<PricingItemDTO>? PricingItems { get; set; } = new List<PricingItemDTO>();
}