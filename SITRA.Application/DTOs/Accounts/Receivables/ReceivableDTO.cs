using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Accounts.Receivables
{
    public class ReceivableDTO
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public SaleInvoiceDTO Invoice { get; set; } = null!;
        public Guid ContactId { get; set; }
        public ContactDTO Buyer { get; set; } = null!;
        public Guid BillerId { get; set; }
        public ContactDTO Biller { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public ICollection<PaymentReceiptListDTO> PaymentReceipts { get; set; } = [];
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
