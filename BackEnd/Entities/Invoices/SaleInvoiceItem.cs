using BackEnd.Entities.Products;

namespace BackEnd.Entities.Invoices
{
    public class SaleInvoiceItem
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public virtual SaleInvoice SaleInvoice { get; set; } = null!;
        public long ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public double Qty { get; set; }
        public double BasePrice { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
    }
}
