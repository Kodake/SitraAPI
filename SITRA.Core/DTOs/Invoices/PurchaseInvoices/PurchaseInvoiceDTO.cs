namespace SITRA.Core.DTOs.Invoices.PurchaseInvoices
{
    using SITRA.Core.DTOs.Contacts.Contacts;
    public class PurchaseInvoiceDTO
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public Guid BillerId { get; set; }
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
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
