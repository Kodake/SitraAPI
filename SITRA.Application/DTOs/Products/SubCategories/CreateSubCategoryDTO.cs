namespace SITRA.Core.DTOs.Products.SubCategories
{
    public class CreateSubCategoryDTO
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
