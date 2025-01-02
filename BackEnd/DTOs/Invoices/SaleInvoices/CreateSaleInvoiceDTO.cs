using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class CreateSaleInvoiceDTO
    {
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public long CustomerId { get; set; }
        public long BillerId { get; set; }
        public DocumentType DocumentType { get; set; }
        public long FiscalSupportId { get; set; }
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double? AmountReceived { get; set; }
        public double? AmountReturned { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = [];
    }
}
