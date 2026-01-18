using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class Model : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public long BrandId { get; set; }
        public Brand Brand { get; set; } = default!;
        public Guid TenantId { get; set; }
    }
}
