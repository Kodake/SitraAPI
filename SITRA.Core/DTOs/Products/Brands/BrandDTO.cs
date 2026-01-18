namespace SITRA.Core.DTOs.Products.Brands
{
    public class BrandDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
