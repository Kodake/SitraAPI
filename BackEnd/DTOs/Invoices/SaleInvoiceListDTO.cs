using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices
{
    public class SaleInvoiceListDTO
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string FiscalSupportNumber { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public double TotalTaxes { get; set; }
        public bool IsPaid { get; set; }
        public InvoiceStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<SaleInvoiceItemDTO> SaleInvoiceItems { get; set; } = new();
    }
}
