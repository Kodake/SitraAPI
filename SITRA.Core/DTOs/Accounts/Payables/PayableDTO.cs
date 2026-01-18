using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.Payables
{
    public class PayableDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public PurchaseInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public ContactDTO Supplier { get; set; } = null!;
        public long BillerId { get; set; }
        public ContactDTO Biller { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public ICollection<PayableReceiptListDTO> PayableReceipts { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
