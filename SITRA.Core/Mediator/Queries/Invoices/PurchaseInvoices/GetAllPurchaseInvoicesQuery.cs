using AutoMapper;
using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
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
    public class GetAllPurchaseInvoicesQuery : IRequest<List<PurchaseInvoiceListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllPurchaseInvoicesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllPurchaseInvoicesQueryHandler : IRequestHandler<GetAllPurchaseInvoicesQuery, List<PurchaseInvoiceListDTO>>
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public GetAllPurchaseInvoicesQueryHandler(IInvoiceService invoiceService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public async Task<List<PurchaseInvoiceListDTO>> Handle(GetAllPurchaseInvoicesQuery request, CancellationToken cancellationToken)
        {
            var purchaseInvoices = await _invoiceService.GetValidPurchaseInvoicesAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<PurchaseInvoiceListDTO>>(purchaseInvoices);
        }
    }
}
