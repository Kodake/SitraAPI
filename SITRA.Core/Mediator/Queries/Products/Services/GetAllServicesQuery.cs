using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Services
{
    public class GetAllServicesQuery : IRequest<List<ServiceListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllServicesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, List<ServiceListDTO>>
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;

        public GetAllServicesQueryHandler(IServiceService serviceService, IMapper mapper)
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        public async ValueTask<List<ServiceListDTO>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var services = await _serviceService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ServiceListDTO>>(services);
        }
    }
}
