using BackEnd.Entities.Contacts;
using BackEnd.Enums;

namespace BackEnd.Entities.Invoices
{
    public class SpotPayment
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public virtual SaleInvoice SaleInvoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual Contact Buyer { get; set; } = null!;
        public double AmountReceived { get; set; }
        public double AmountReturned { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
