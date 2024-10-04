using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public sealed class SaleInvoice
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public double TotalAmount
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.TotalPrice);
            }
        }
        public double TotalTaxes
        {
            get
            {
                return SaleInvoiceItems.Sum(x => x.ItemTaxes.Sum(it => it.Tax.Amount * x.Qty));
            }
        }
        public bool IsPaid { get; set; } = false;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentMethod PaymentMethod { get; set; }
        public FiscalSupport FiscalSupport { get; set; } = null!;
        public Deadline Deadline { get; set; } = null!;
        public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = [];
    }
}
