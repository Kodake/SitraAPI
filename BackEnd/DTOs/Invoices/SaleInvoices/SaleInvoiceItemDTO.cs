using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceItemDTO
    {
        public long ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public double Qty { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = new List<ItemTaxes>();
    }
}
