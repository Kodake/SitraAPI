using BackEnd.Entities.Accounts;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public class SaleInvoice
    {
        public long Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public long PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public long BillerId { get; set; }
        public Contact Biller { get; set; } = null!;
        public long CustomerId { get; set; }
        public Contact Buyer { get; set; } = null!;
        public long FiscalSupportId { get; set; }
        public FiscalSupport FiscalSupport { get; set; } = null!;
        public long CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public InvoiceCondition ConditionType { get; set; }
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public string? Note { get; set; } = null!;
        public DateTime? DueDate { get; set; } = null!;
        public Receivable? Receivable { get; set; } = null!;
        public SpotPayment? SpotPayment { get; set; } = null!;
        public List<SaleInvoiceItem> SaleInvoiceItems { get; set; } = new List<SaleInvoiceItem>();
    }
}
