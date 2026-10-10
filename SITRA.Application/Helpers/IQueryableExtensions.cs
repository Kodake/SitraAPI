using SITRA.Core.DTOs;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Enums;

namespace SITRA.Core.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable.Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
                .Take(paginationDTO.RecordsPerPage);
        }

        public static IQueryable<Invoice> WhereValidSaleInvoice(this IQueryable<Invoice> queryable)
        {
            return queryable.Where(x =>
                x.InvoiceType == InvoiceType.Sale
                && x.Status != InvoiceStatus.Rejected
                && x.Status != InvoiceStatus.Failed
                && x.Status != InvoiceStatus.Cancelled
                && x.DgiiStatus != "Rechazado"
                && x.DgiiStatus != "Error"
                && (x.SpotPayment != null || x.Receivable != null));
        }

        public static IQueryable<Invoice> WhereValidPurchaseInvoice(this IQueryable<Invoice> queryable)
        {
            return queryable.Where(x =>
                x.InvoiceType == InvoiceType.Purchase
                && x.Status != InvoiceStatus.Rejected
                && x.Status != InvoiceStatus.Failed
                && x.Status != InvoiceStatus.Cancelled
                && x.DgiiStatus != "Rechazado"
                && x.DgiiStatus != "Error"
                && (x.SpotPayable != null || x.Payable != null));
        }
    }
}

