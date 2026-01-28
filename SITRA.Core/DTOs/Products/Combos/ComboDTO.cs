using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.Combos;

//using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.DTOs.Products.Units;

public class ComboDTO
{
    public Guid Id { get; set; }
    public List<ItemQtyDTO> Items { get; set; } = [];
    public List<ComboItemDTO>? ComboItems { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public double InitialCost { get; set; }
    public double BasePrice { get; set; }
    public double Percentage { get; set; }
    public int? BrandId { get; set; }
    public BrandDTO? Brand { get; set; }
    public Guid? ModelId { get; set; }
    public ModelDTO? Model { get; set; }
    public int? CategoryId { get; set; }
    public CategoryDTO? Category { get; set; }
    public Guid? SubCategoryId { get; set; }
    public SubCategoryDTO? SubCategory { get; set; }
    public int? UnitId { get; set; }
    public UnitDTO? Unit { get; set; }
    public List<int>? TaxIds { get; set; }
    public List<TaxDTO>? Taxes { get; set; } = [];
    public List<PricingItemDTO>? PricingItems { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
