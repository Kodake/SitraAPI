using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Pricings;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Pricings
{
    public class GetUnregisteredPricingsForItemQuery : IRequest<IEnumerable<PricingListDTO>>
    {
        public Guid Id { get; set; }

        public GetUnregisteredPricingsForItemQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetUnregisteredPricingsForItemQueryHandler : IRequestHandler<GetUnregisteredPricingsForItemQuery, IEnumerable<PricingListDTO>>
    {
        private readonly IPricingService _pricingService;
        private readonly IMapper _mapper;

        public GetUnregisteredPricingsForItemQueryHandler(IPricingService pricingService, IMapper mapper)
        {
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async ValueTask<IEnumerable<PricingListDTO>> Handle(GetUnregisteredPricingsForItemQuery request, CancellationToken cancellationToken)
        {
            var pricings = await _pricingService.GetUnregisteredPricingsForItemAsync(request.Id, cancellationToken);
            return _mapper.Map<IEnumerable<PricingListDTO>>(pricings);
        }
    }
}
