using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Products;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public double InitialCost
    {
        get
        {
            return PricingItems?.FirstOrDefault(p => p.IsPrincipal)?.InitialCost ?? 0;
        }
    }
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
    public int? BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
    public int? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public int? UnitId { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<int>? TaxIds
    {
        get
        {
            return Taxes?.Select(x => x.Id).ToList();
        }
    }
    public List<TaxDTO>? Taxes { get; set; } = new List<TaxDTO>();
    public List<PricingItemDTO>? PricingItems { get; set; } = new List<PricingItemDTO>();
}
