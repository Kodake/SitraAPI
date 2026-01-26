using SITRA.Core.Interfaces;
using AutoMapper;
using MediatR;
using SITRA.Core.DTOs.Invoices.SaleInvoices;

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
        private readonly IUnitOfWork _unitOfWork;

        public GetSaleInvoicesBalanceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BalanceSummaryDTO> Handle(GetSaleInvoicesBalanceQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.SaleInvoiceRepository.GetBalanceAsync(request.From, request.To);
        }
    }
}
