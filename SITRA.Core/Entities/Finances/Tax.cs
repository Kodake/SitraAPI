using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Finances
{
    public sealed class Tax : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
        public Guid TenantId { get; set; }
    }
}
