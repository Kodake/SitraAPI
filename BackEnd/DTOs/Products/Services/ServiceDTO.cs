using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.Categories;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.DTOs.Products.Units;

public class ServiceDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public int? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public int? SubCategoryId { get; set; }
    public SubCategoryDTO? SubCategory { get; set; }
    public int? UnitId { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<int>? TaxIds { get; set; }
    public List<TaxDTO>? Taxes { get; set; } = [];
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
}
