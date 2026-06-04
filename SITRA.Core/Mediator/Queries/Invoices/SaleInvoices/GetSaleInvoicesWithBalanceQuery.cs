using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
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
        private readonly IFinanceService _financeService;

        public GetSaleInvoicesWithBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFinanceService financeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _financeService = financeService;
        }

        public async ValueTask<SaleInvoiceWithBalanceDTO> Handle(GetSaleInvoicesWithBalanceQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _unitOfWork.SaleInvoiceRepository.GetSalesByDateRangeAsync(request.From, request.To, request.Pagination, cancellationToken);
            var balance = await _financeService.GetSaleBalanceAsync(request.From, request.To, cancellationToken);

            return new SaleInvoiceWithBalanceDTO
            {
                Invoices = _mapper.Map<List<SaleInvoiceListDTO>>(invoices),
                Balance = balance
            };
        }
    }
}
