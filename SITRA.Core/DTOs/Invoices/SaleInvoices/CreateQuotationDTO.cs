namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class CreateQuotationDTO
    {
        public Guid PricingId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BillerId { get; set; }
        public DateTime? DueDate { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public List<SaleInvoiceItemDTO> InvoiceItems { get; set; } = [];
    }
}
