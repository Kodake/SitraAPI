using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Enums;

namespace BackEnd.DTOs.Invoices.SpotPayables
{
    public class SpotPayableDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public virtual PurchaseInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual ContactDTO Supplier { get; set; } = null!;
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
