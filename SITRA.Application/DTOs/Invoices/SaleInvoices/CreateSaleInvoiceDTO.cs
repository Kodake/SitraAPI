using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class CreateSaleInvoiceDTO
    {
        public Guid Id { get; set; }
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? BillerId { get; set; }
        public DocumentType? DocumentType { get; set; }
        public Guid? FiscalSupportId { get; set; }
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public InvoiceCondition ConditionType { get; set; }
        public List<PaymentDTO> Payments { get; set; } = [];
        public double? AmountReturned { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = [];
        public double? MontoNoFacturable { get; set; }
        public double? SaldoAnterior { get; set; }
        public double? MontoAvancePago { get; set; }
        public double? MontoPeriodo { get; set; }
        public string? TipoIngresos { get; set; }
        public int? IndicadorEnvioDiferido { get; set; }
        public string? IndicadorMontoGral { get; set; }
    }
}
