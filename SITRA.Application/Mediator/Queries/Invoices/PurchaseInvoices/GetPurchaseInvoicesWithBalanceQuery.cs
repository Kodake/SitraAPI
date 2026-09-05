using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
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
        private readonly IFinanceService _financeService;

        public GetPurchaseInvoicesWithBalanceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IFinanceService financeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _financeService = financeService;
        }

        public async ValueTask<PurchaseInvoiceWithBalanceDTO> Handle(GetPurchaseInvoicesWithBalanceQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _unitOfWork.PurchaseInvoiceRepository
                .GetPurchasesByDateRangeAsync(request.From, request.To, request.Pagination, cancellationToken);

            var invoicesDto = _mapper.Map<List<PurchaseInvoiceListDTO>>(invoices);

            var balance = await _financeService
                .GetPurchaseBalanceAsync(request.From, request.To, cancellationToken);

            return new PurchaseInvoiceWithBalanceDTO
            {
                Invoices = invoicesDto,
                Balance = balance
            };
        }
    }
}
