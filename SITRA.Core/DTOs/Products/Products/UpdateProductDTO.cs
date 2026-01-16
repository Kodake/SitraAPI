using Microsoft.AspNetCore.Http;
namespace SITRA.Core.DTOs.Products.Products
{
    public class UpdateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageUrl { get; set; }
        public double InitialCost { get; set; }
        public double BasePrice { get; set; }
        public List<long> TaxIds { get; set; } = null!;
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public long? UnitId { get; set; }
    }
}

