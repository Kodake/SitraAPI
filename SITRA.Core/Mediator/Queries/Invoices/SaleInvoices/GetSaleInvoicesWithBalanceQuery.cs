using AutoMapper;
using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;

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

    public class GetSaleInvoicesWithBalanceQueryHandler : IRequestHandler<GetSaleInvoicesWithBalanceQuery, SaleInvoiceWithBalanceDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSaleInvoicesWithBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SaleInvoiceWithBalanceDTO> Handle(GetSaleInvoicesWithBalanceQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _unitOfWork.SaleInvoiceRepository.GetSalesByDateRangeAsync(request.From, request.To, request.Pagination);
            var balance = await _unitOfWork.SaleInvoiceRepository.GetBalanceAsync(request.From, request.To);

            return new SaleInvoiceWithBalanceDTO
            {
                Invoices = _mapper.Map<List<SaleInvoiceListDTO>>(invoices),
                Balance = balance
            };
        }
    }
}
