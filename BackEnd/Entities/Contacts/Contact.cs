using BackEnd.Entities.Accounts;
using BackEnd.Entities.Invoices;
using BackEnd.Enums;

namespace BackEnd.Entities.Contacts
{
    public sealed class Contact
    {
        public long Id { get; set; }
        public ContactType? ContactType { get; set; }
        public IdentificationType? IdentificationType { get; set; }
        public string? Identification { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<SaleInvoice> BillerInvoices { get; set; } = new List<SaleInvoice>();
        public ICollection<SaleInvoice> BuyerInvoices { get; set; } = new List<SaleInvoice>();
        public ICollection<Receivable> Receivables { get; set; } = new List<Receivable>();
    }
}
