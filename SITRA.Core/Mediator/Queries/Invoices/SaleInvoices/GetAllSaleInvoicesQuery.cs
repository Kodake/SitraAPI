using AutoMapper;
using MediatR;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
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
    public class GetAllSaleInvoicesQuery : IRequest<List<SaleInvoiceListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllSaleInvoicesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllSaleInvoicesQueryHandler : IRequestHandler<GetAllSaleInvoicesQuery, List<SaleInvoiceListDTO>>
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public GetAllSaleInvoicesQueryHandler(IInvoiceService invoiceService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public async Task<List<SaleInvoiceListDTO>> Handle(GetAllSaleInvoicesQuery request, CancellationToken cancellationToken)
        {
            var saleInvoices = await _invoiceService.GetValidSaleInvoicesAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);
        }
    }
}
