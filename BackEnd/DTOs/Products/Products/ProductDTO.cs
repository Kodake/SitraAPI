using BackEnd.DTOs.Finances.Taxes;
using BackEnd.DTOs.Products.Brands;
using BackEnd.DTOs.Products.Categories;
using BackEnd.DTOs.Products.Models;
using BackEnd.DTOs.Products.PricingItems;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.DTOs.Products.Units;

public class ProductDTO
{
    public long Id { get; set; }
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
    public int? ModelId { get; set; }
    public ModelDTO? Model { get; set; }
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
