using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SaleInvoices
{
    public class PaymentDTO
    {
        public PaymentMethod PaymentId { get; set; }
        public double Amount { get; set; }
    }
}
