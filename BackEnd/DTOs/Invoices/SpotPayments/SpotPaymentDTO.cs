using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Contacts;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SpotPayments
{
    public class SpotPaymentDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual SaleInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Buyer { get; set; } = null!;
        public double AmountReceived { get; set; }
        public double AmountReturned { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
