using SITRA.Core.DTOs.Products.Categories;

namespace SITRA.Core.DTOs.Products.SubCategories
{
    public class SubCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDTO Category { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
