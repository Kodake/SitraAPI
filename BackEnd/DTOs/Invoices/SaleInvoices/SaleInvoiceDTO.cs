namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class SaleInvoiceDTO
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long BillerId { get; set; }
        public long PricingId { get; set; }
        public long CurrencyId { get; set; }
        public long WarehouseId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DueDate { get; set; }
        public double? Shipping { get; set; } = null!;
        public double? Discount { get; set; } = null!;
        public double TotalPrice { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
    }
}
