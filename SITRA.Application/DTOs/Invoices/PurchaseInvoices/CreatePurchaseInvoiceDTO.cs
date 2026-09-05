using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.PurchaseInvoices
{
    public class CreatePurchaseInvoiceDTO
    {
        public Guid Id { get; set; }
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid SupplierId { get; set; }
        public DocumentType? DocumentType { get; set; }
        public Guid? FiscalSupportId { get; set; }
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public List<PurchaseInvoiceItemDTO> InvoiceItems { get; set; } = [];
    }
}
