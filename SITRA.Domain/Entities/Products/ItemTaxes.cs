using System.Text.Json.Serialization;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.Products
{
    public sealed class ItemTaxes : ITenant, IAuditableEntity
    {
        public Guid ItemId { get; set; }
        public Guid TaxId { get; set; }
        
        [JsonIgnore]
        public Item Item { get; set; } = null!;
        public Tax Tax { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
