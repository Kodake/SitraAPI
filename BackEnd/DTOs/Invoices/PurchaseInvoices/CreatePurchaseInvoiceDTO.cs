using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.PurchaseInvoices
{
    public class CreatePurchaseInvoiceDTO
    {
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public long SupplierId { get; set; }
        public long BillerId { get; set; }
        public DocumentType DocumentType { get; set; }
        public long? FiscalSupportId { get; set; }
        public string? FiscalSupportNumber { get; set; } = string.Empty;
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public double? AmountReceived { get; set; }
        public double? AmountReturned { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public List<PurchaseInvoiceItemDTO> InvoiceItems { get; set; } = [];
    }
}
