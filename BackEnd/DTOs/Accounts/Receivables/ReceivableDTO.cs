using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts.Receivables
{
    public class ReceivableDTO
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public SaleInvoiceDTO Invoice { get; set; } = null!;
        public long ContactId { get; set; }
        public ContactDTO Buyer { get; set; } = null!;
        public long BillerId { get; set; }
        public ContactDTO Biller { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public ICollection<PaymentReceiptListDTO> PaymentReceipts { get; set; } = [];
    }
}
