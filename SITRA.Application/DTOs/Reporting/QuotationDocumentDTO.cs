using SITRA.Core.Entities.Invoices;

namespace SITRA.Core.DTOs.Reporting
{
    public class QuotationDocumentDTO
    {
        public QuotationDocumentDTO(Invoice invoice)
        {
            Invoice = invoice;
        }

        public Invoice Invoice { get; }
    }
}
