using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Enums;

public class ServiceDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public Guid? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public Guid? SubCategoryId { get; set; }
    public SubCategoryDTO? SubCategory { get; set; }
    public UnitMeasure? UnitMeasure { get; set; }
    public List<Guid>? TaxIds { get; set; }
    public List<TaxDTO>? Taxes { get; set; } = [];
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
