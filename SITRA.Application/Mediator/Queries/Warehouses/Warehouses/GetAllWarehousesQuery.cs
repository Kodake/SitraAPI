using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Interfaces.Services.Warehouses;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
{
    public class GetAllWarehousesQuery : IRequest<List<WarehouseListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllWarehousesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, List<WarehouseListDTO>>
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IMapper _mapper;

        public GetAllWarehousesQueryHandler(IWarehouseService warehouseService, IMapper mapper)
        {
            _warehouseService = warehouseService;
            _mapper = mapper;
        }

        public async ValueTask<List<WarehouseListDTO>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<WarehouseListDTO>>(warehouses);
        }
    }
}
