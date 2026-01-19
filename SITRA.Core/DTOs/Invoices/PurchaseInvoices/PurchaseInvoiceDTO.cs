namespace SITRA.Core.DTOs.Invoices.PurchaseInvoices
{
    using SITRA.Core.DTOs.Contacts.Contacts;
    public class PurchaseInvoiceDTO
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long BillerId { get; set; }
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public double TotalPrice { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public ContactDTO? Supplier { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
