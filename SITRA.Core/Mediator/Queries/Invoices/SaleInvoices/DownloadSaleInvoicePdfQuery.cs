using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class DownloadSaleInvoicePdfQuery : GetSaleInvoicePdfQuery
    {
        public DownloadSaleInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings) 
            : base(invoiceId, dgiiSettings) { }
    }
}
