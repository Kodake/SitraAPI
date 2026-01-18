using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Brand : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public ICollection<Item> Items { get; set; } = [];
        public ICollection<Model> Models { get; set; } = [];
        public Guid TenantId { get; set; }
    }
}
