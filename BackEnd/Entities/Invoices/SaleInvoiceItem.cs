using BackEnd.Entities.Products;

namespace BackEnd.Entities.Invoices
{
    public sealed class SaleInvoiceItem
    {
        public int Id { get; set; }
        public Guid SaleInvoiceId { get; set; }
        public SaleInvoice? SaleInvoice { get; set; }
        public int ItemId { get; set; }
        public Item? Item { get; set; }
        public double Qty { get; set; }
        public ICollection<ItemTaxes> ItemTaxes { get; set; } = new List<ItemTaxes>();
        public double BasePrice
        {
            get
            {
                return Item?.PricingItems.FirstOrDefault(pi => pi.IsPrincipal)?.BasePrice ?? 0;
            }
        }
        public double TotalPrice
        {
            get
            {
                double totalTaxes = ItemTaxes.Sum(it => it.Tax.Amount);
                return BasePrice * Qty + (BasePrice * totalTaxes / 100 * Qty);
            }
        }
    }
}
