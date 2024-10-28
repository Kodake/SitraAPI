using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Finances;
using BackEnd.Entities.Products;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public sealed class SaleInvoice
    {
        public long Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int PricingId { get; set; }
        public Pricing Pricing { get; set; } = null!;
        public int BillerId { get; set; }
        public Contact Biller { get; set; } = null!;
        public int BuyerId { get; set; }
        public Contact Buyer { get; set; } = null!;
        public int FiscalSupportId { get; set; }
        public FiscalSupport FiscalSupport { get; set; } = null!;
        public int DeadlineId { get; set; }
        public Deadline Deadline { get; set; } = null!;
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public double DeliveryFees { get; set; }
        public double TotalQtyItems { get; set; }
        public double TotalItems { get; set; }
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public double PaymentReceived { get; set; }
        public double PaymentReturned { get; set; }
        public bool IsPaid => PaymentReceived >= TotalAmount;
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ICollection<SaleInvoiceItem> SaleInvoiceItems { get; set; } = [];
    }
}
