using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.DTOs.Products.Units;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public Guid? BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
    public Guid? ModelId { get; set; }
    public ModelDTO? Model { get; set; }
    public Guid? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public Guid? SubCategoryId { get; set; }
    public SubCategoryDTO? SubCategory { get; set; }
    public Guid? UnitId { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<Guid>? TaxIds { get; set; }
    public List<TaxDTO>? Taxes { get; set; } = [];
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
