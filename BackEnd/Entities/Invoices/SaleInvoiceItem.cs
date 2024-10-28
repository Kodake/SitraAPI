using BackEnd.Entities.Products;

namespace BackEnd.Entities.Invoices
{
    public sealed class SaleInvoiceItem
    {
        public int Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public SaleInvoice SaleInvoice { get; set; } = null!;
        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public double Qty { get; set; }
        public double BasePrice { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
    }
}
