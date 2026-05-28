using AutoMapper;
using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices
{
    public class GetPurchaseInvoicesWithBalanceQuery : IRequest<PurchaseInvoiceWithBalanceDTO>
    {
        public DateTime From { get; }
        public DateTime To { get; }
        public PaginationDTO Pagination { get; }

        public GetPurchaseInvoicesWithBalanceQuery(DateTime from, DateTime to, PaginationDTO pagination)
        {
            From = from;
            To = to;
            Pagination = pagination;
        }
    }

    public class GetPurchaseInvoicesWithBalanceQueryHandler : IRequestHandler<GetPurchaseInvoicesWithBalanceQuery, PurchaseInvoiceWithBalanceDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPurchaseInvoicesWithBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PurchaseInvoiceWithBalanceDTO> Handle(GetPurchaseInvoicesWithBalanceQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _unitOfWork.PurchaseInvoiceRepository
                .GetPurchasesByDateRangeAsync(request.From, request.To, request.Pagination, cancellationToken);

            var invoicesDto = _mapper.Map<List<PurchaseInvoiceListDTO>>(invoices);

            var balance = await _unitOfWork.PurchaseInvoiceRepository
                .GetBalanceAsync(request.From, request.To, cancellationToken);

            return new PurchaseInvoiceWithBalanceDTO
            {
                Invoices = invoicesDto,
                Balance = balance
            };
        }
    }
}
