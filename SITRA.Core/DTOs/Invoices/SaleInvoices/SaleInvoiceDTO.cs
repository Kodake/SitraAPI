namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    using SITRA.Core.DTOs.Contacts.Contacts;
    using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
    public class SaleInvoiceDTO
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long BillerId { get; set; }
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public double TotalAmount { get; set; }
        public double TotalPrice { get; set; }
        public double TotalTaxes { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public ContactDTO? Buyer { get; set; }
        public ContactDTO? Biller { get; set; }
        public FiscalSupportDTO? FiscalSupport { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
