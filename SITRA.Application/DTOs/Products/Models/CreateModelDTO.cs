namespace SITRA.Core.DTOs.Products.Models
{
    public class CreateModelDTO
    {
        public Guid BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
