using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
{
    public class GetAllPricingsQuery : IRequest<List<PricingListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllPricingsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllPricingsQueryHandler : IRequestHandler<GetAllPricingsQuery, List<PricingListDTO>>
    {
        private readonly IPricingService _pricingService;
        private readonly IMapper _mapper;

        public GetAllPricingsQueryHandler(IPricingService pricingService, IMapper mapper)
        {
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async ValueTask<List<PricingListDTO>> Handle(GetAllPricingsQuery request, CancellationToken cancellationToken)
        {
            var pricings = await _pricingService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<PricingListDTO>>(pricings);
        }
    }
}
