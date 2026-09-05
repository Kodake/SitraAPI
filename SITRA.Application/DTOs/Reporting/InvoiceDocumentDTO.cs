using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.DTOs.Reporting
{
    public class InvoiceDocumentDTO
    {
        public InvoiceDocumentDTO(Invoice invoice)
        {
            Invoice = invoice;
        }

        public Invoice Invoice { get; }
    }
}
