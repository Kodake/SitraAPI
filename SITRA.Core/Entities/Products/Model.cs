using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public class Model : ITenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public long BrandId { get; set; }
        public Brand Brand { get; set; } = default!;
        public Guid TenantId { get; set; }
    }
}
