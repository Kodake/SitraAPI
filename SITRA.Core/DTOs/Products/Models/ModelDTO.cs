using SITRA.Core.DTOs.Products.Brands;

namespace SITRA.Core.DTOs.Products.Models
{
    public class ModelDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long BrandId { get; set; }
        public BrandDTO Brand { get; set; } = default!;
    }
}
