using SITRA.Core.Entities.Auths;
using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.DTOs.Reporting
{
    public class InvoiceReceiptDocumentDTO
    {
        public InvoiceReceiptDocumentDTO(Invoice invoice, User user)
        {
            Invoice = invoice;
            User = user;
        }

        public Invoice Invoice { get; }
        public User User { get; }
    }
}
