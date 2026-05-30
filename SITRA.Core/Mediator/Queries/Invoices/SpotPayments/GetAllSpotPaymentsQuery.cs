using MediatR;
using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Invoices.SpotPayments
{
    public class GetAllSpotPaymentsQuery : IRequest<List<SpotPaymentListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllSpotPaymentsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllSpotPaymentsQueryHandler : IRequestHandler<GetAllSpotPaymentsQuery, List<SpotPaymentListDTO>>
    {
        private readonly SITRA.Core.Interfaces.Services.Invoices.IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public GetAllSpotPaymentsQueryHandler(SITRA.Core.Interfaces.Services.Invoices.IInvoiceService invoiceService, IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        public async Task<List<SpotPaymentListDTO>> Handle(GetAllSpotPaymentsQuery request, CancellationToken cancellationToken)
        {
            var spotPayments = await _invoiceService.GetValidSpotPaymentsAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<SpotPaymentListDTO>>(spotPayments);
        }
    }
}
