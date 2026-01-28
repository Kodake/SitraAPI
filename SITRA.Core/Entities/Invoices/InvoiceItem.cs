using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Products;

namespace SITRA.Core.Entities.Invoices
{
    public class InvoiceItem : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public double Qty { get; set; }
        public double BasePrice { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
    }
}
