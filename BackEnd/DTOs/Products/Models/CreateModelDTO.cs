namespace BackEnd.DTOs.Products.Models
{
    public class CreateModelDTO
    {
        public long BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
