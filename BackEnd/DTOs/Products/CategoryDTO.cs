namespace BackEnd.DTOs.Products
{
    public class CategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
