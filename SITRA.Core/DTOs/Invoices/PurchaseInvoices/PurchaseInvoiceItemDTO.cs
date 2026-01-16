using SITRA.Core.Entities.Products;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.PurchaseInvoices
{
    public class PurchaseInvoiceItemDTO
    {
        public long ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public double Qty { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = new List<ItemTaxes>();
    }
}
