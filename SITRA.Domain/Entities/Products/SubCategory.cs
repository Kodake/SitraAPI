using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class SubCategory : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public Guid TenantId { get; set; }
    }
}
