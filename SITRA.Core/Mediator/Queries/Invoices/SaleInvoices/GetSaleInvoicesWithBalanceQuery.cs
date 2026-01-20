using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicesWithBalanceQuery : IRequest<SaleInvoiceWithBalanceDTO>
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public PaginationDTO Pagination { get; }

        public GetSaleInvoicesWithBalanceQuery(DateTime from, DateTime to, PaginationDTO pagination)
        {
            From = from;
            To = to;
            Pagination = pagination;
        }
    }
}
