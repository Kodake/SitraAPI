using MediatR;
using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayables
{
    public class GetAllSpotPayablesQuery : IRequest<List<SpotPayableListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllSpotPayablesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllSpotPayablesQueryHandler : IRequestHandler<GetAllSpotPayablesQuery, List<SpotPayableListDTO>>
    {
        private readonly SITRA.Core.Interfaces.Services.Invoices.IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public GetAllSpotPayablesQueryHandler(SITRA.Core.Interfaces.Services.Invoices.IInvoiceService invoiceService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public async Task<List<SpotPayableListDTO>> Handle(GetAllSpotPayablesQuery request, CancellationToken cancellationToken)
        {
            var spotPayables = await _invoiceService.GetValidSpotPayablesAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<SpotPayableListDTO>>(spotPayables);
        }
    }
}
