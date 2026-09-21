using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.Enums;
using System.Text.Json.Serialization;

namespace SITRA.Core.DTOs.Products.Services;
public class ServiceListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType ItemType { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public double TotalPrice { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<TaxDTO> Taxes { get; set; } = [];
    [JsonIgnore]
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
