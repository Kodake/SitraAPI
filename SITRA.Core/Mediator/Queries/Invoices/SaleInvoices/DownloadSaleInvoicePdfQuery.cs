using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class DownloadSaleInvoicePdfQuery : GetSaleInvoicePdfQuery
    {
        public DownloadSaleInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings, SITRA.Core.Interfaces.Services.IDgiiStorageService dgiiStorageService) 
            : base(invoiceId, dgiiSettings, dgiiStorageService) { }
    }
}
