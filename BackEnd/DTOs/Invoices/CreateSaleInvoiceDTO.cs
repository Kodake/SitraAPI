using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices
{
    public class CreateSaleInvoiceDTO
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsPaid { get; set; } = false;
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int FiscalSupportId { get; set; }
        public int DeadlineId { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = [];
    }
}
