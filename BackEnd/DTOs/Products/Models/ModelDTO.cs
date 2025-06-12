using BackEnd.DTOs.Products.Brands;

namespace BackEnd.DTOs.Products.Models
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
