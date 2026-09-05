using Mediator;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetSaleInvoicesBalanceQuery : IRequest<BalanceSummaryDTO>
    {
        public DateTimeOffset From { get; }
        public DateTimeOffset To { get; }

        public GetSaleInvoicesBalanceQuery(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }

    public class GetSaleInvoicesBalanceQueryHandler : IRequestHandler<GetSaleInvoicesBalanceQuery, BalanceSummaryDTO>
    {
        private readonly IFinanceService _financeService;

        public GetSaleInvoicesBalanceQueryHandler(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        public async ValueTask<BalanceSummaryDTO> Handle(GetSaleInvoicesBalanceQuery request, CancellationToken cancellationToken)
        {
            return await _financeService.GetSaleBalanceAsync(request.From, request.To, cancellationToken);
        }
    }
}
