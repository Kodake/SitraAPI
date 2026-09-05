using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Category : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; } = [];
    }
}
