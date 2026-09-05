using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Interfaces.Services.Invoices;

namespace SITRA.Core.Mediator.Queries.Invoices.SaleInvoices
{
    public class GetAllSaleInvoicesByDateRangeAndBillerQuery : IRequest<List<SaleInvoiceListDTO>>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public PaginationDTO Pagination { get; set; }

        public GetAllSaleInvoicesByDateRangeAndBillerQuery(DateTime from, DateTime to, PaginationDTO pagination)
        {
            From = from;
            To = to;
            Pagination = pagination;
        }
    }

    public class GetAllSaleInvoicesByDateRangeAndBillerQueryHandler : IRequestHandler<GetAllSaleInvoicesByDateRangeAndBillerQuery, List<SaleInvoiceListDTO>>
    {
        private readonly ISaleInvoiceService _saleInvoiceService;
        private readonly IMapper _mapper;

        public GetAllSaleInvoicesByDateRangeAndBillerQueryHandler(ISaleInvoiceService saleInvoiceService, IMapper mapper)
        {
            _saleInvoiceService = saleInvoiceService;
            _mapper = mapper;
        }

        public async ValueTask<List<SaleInvoiceListDTO>> Handle(GetAllSaleInvoicesByDateRangeAndBillerQuery request, CancellationToken cancellationToken)
        {
            var saleInvoices = await _saleInvoiceService.GetSalesByDateRangeAsync(request.From, request.To, request.Pagination, cancellationToken);
            return _mapper.Map<List<SaleInvoiceListDTO>>(saleInvoices);
        }
    }
}
