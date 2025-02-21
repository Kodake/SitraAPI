using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SaleInvoices
{
    public class PaymentDTO
    {
        public PaymentMethod PaymentId { get; set; }
        public double Amount { get; set; }
    }
}
