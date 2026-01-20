using AutoMapper;
using MediatR;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries.Invoices.SaleInvoices;

namespace SITRA.Core.Mediator.Handlers.Invoices.SaleInvoices
{
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
            // Get invoices (this will also add pagination headers via the repository/middleware)
            var saleInvoices = await _unitOfWork.SaleInvoiceRepository
                .GetSalesByDateRangeAsync(request.From, request.To, request.Pagination);

            // Get balance
            var balance = await _unitOfWork.SaleInvoiceRepository
                .GetBalanceAsync(request.From, request.To);

            var saleInvoiceDtos = _mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);

            return new SaleInvoiceWithBalanceDTO
            {
                Invoices = saleInvoiceDtos,
                Balance = balance
            };
        }
    }
}
