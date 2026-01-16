namespace SITRA.Core.DTOs.Products.Models
{
    public class ModelListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long BrandId { get; set; }
        public string BrandName { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
