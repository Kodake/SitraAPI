using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Products.Combos
{
    public class CreateComboDTO
    {
        public List<ItemQtyDTO> Items { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public List<Guid> TaxIds { get; set; } = null!;
        public Guid? BrandId { get; set; }
        public Guid? ModelId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubCategoryId { get; set; }
        public UnitMeasure? UnitMeasure { get; set; }
    }
}

