using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.Quotations;
using SITRA.Core.Interfaces.Services.Invoices;

namespace SITRA.Core.Mediator.Queries.Invoices.Quotations
{
    public class GetAllQuotationsQuery : IRequest<List<QuotationListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllQuotationsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllQuotationsQueryHandler : IRequestHandler<GetAllQuotationsQuery, List<QuotationListDTO>>
    {
        private readonly IQuotationService _quotationService;
        private readonly IMapper _mapper;

        public GetAllQuotationsQueryHandler(IQuotationService quotationService, IMapper mapper)
        {
            _quotationService = quotationService;
            _mapper = mapper;
        }

        public async ValueTask<List<QuotationListDTO>> Handle(GetAllQuotationsQuery request, CancellationToken cancellationToken)
        {
            var quotations = await _quotationService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<QuotationListDTO>>(quotations);
        }
    }
}
