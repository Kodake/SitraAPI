using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SpotPayments
{
    public class SpotPaymentDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual SaleInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Buyer { get; set; } = null!;
        public virtual ICollection<PaymentDTO> Payments { get; set; } = [];
        public double AmountReceived { get; set; }
        public double AmountReturned { get; set; }
        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
