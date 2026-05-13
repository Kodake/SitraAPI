using SITRA.Core.Models.Dgii;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class DownloadPurchaseInvoicePdfQuery : GetPurchaseInvoicePdfQuery
    {
        public DownloadPurchaseInvoicePdfQuery(Guid invoiceId, DgiiSettings dgiiSettings) 
            : base(invoiceId, dgiiSettings) { }
    }
}
