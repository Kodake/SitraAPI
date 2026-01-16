using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Category : ITenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<Item> Items { get; set; } = [];
        public ICollection<SubCategory> SubCategories { get; set; } = [];
    }
}
