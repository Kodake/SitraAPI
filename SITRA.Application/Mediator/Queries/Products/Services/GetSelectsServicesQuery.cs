using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Services
{
    public class GetSelectsServicesQuery : IRequest<List<ServiceListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsServicesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsServicesQueryHandler : IRequestHandler<GetSelectsServicesQuery, List<ServiceListDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public GetSelectsServicesQueryHandler(IServiceService serviceService, IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async ValueTask<List<ServiceListDTO>> Handle(GetSelectsServicesQuery request, CancellationToken cancellationToken)
        {
            var services = await _serviceService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<ServiceListDTO>>(services);
        }
    }
}
