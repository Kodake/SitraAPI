using BackEnd.DTOs.Contacts;
using BackEnd.DTOs.Invoices;
using BackEnd.Enums;

namespace BackEnd.DTOs.Accounts
{
    public class ReceivableListDTO
    {
        public long Id { get; set; }
        public long SaleInvoiceId { get; set; }
        public SaleInvoiceDTO SaleInvoice { get; set; } = null!;
        public long ContactId { get; set; }
        public ContactDTO Buyer { get; set; } = null!;
        public double TotalAmountDue { get; set; }
        public double TotalOutstanding { get; set; }
        public AccountStatus Status { get; set; }
    }
}
