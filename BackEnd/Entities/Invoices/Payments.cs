using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public class Payment
    {
        public long Id { get; set; }
        public long SpotPaymentId { get; set; }
        public SpotPayment SpotPayment { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}
