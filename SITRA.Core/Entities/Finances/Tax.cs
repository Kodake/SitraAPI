using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Products;

namespace SITRA.Core.Entities.Finances
{
    public sealed class Tax : ITenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
        public Guid TenantId { get; set; }
    }
}
