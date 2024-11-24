using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Products;

public class ProductDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public int? BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
    public int? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public int? UnitId { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<int>? TaxIds { get; set; }
    public List<TaxDTO>? Taxes { get; set; } = [];
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
}
