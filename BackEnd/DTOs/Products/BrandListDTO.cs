namespace BackEnd.DTOs.Products
{
public class BrandListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}