using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices
{
    public class CreateSaleInvoiceDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int PricingId { get; set; }
        public int BillerId { get; set; }
        public int BuyerId { get; set; }
        public int FiscalSupportId { get; set; }
        public int DeadlineId { get; set; }
        public int CurrencyId { get; set; }
        public double DeliveryFees { get; set; }
        public double PaymentReceived { get; set; }
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = [];
    }
}
