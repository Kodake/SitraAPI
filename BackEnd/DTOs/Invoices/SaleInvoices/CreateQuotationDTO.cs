namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class CreateQuotationDTO
    {
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public long CustomerId { get; set; }
        public long BillerId { get; set; }
        public DateTime? DueDate { get; set; } = null!;
        public string QuotationNumber { get; set; } = string.Empty;
        public double? Shipping { get; set; }
        public double? Discount { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = [];
    }
}
