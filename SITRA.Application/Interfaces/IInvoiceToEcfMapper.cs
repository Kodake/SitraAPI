using SITRA.Core.Entities.Invoices;
using SITRA.Core.DTOs.DGII;

namespace SITRA.Core.Interfaces
{
    public interface IInvoiceToEcfMapper
    {
        EcfDocument MapToDgiiDocument(Invoice invoice);
    }
}
