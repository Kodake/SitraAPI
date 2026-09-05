namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
    using SITRA.Core.DTOs.Contacts.Contacts;
    public class SaleInvoiceDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BillerId { get; set; }
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
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
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
    }
}
