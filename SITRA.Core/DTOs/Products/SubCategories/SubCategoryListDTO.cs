namespace SITRA.Core.DTOs.Products.SubCategories
{
    public class SubCategoryListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
