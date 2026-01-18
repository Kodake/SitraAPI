using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class Unit : ITenant, IAuditableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<Item> Items { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
