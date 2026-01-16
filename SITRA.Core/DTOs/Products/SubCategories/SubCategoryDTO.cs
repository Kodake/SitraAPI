using SITRA.Core.DTOs.Products.Categories;

namespace SITRA.Core.DTOs.Products.SubCategories
{
    public class SubCategoryDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public CategoryDTO Category { get; set; } = default!;
    }
}
