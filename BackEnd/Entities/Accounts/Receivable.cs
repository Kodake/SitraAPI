using BackEnd.Entities.Contacts;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;

namespace BackEnd.Entities.Accounts
{
    public class Receivable
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public virtual SaleInvoice SaleInvoice { get; set; } = null!;
        public long ContactId { get; set; }
        public virtual Contact Buyer { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
        public ICollection<PaymentReceipt> PaymentReceipts { get; set; } = [];
    }
}
