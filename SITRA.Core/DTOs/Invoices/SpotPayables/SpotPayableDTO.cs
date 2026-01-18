using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Invoices.SpotPayables
{
    public class SpotPayableDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual PurchaseInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Supplier { get; set; } = null!;
        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
