using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.Enums;
using System.Text.Json.Serialization;

namespace BackEnd.DTOs.Products.Services;
public class ServiceListDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public double TotalPrice { get; set; }
    public bool IsActive { get; set; }
    public List<TaxDTO> Taxes { get; set; } = [];
    [JsonIgnore]
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
}