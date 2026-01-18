using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceItemDTO
    {
        public long ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public double Qty { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
