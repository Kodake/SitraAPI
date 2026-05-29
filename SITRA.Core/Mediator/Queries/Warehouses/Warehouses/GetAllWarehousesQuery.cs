using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
{
    public class GetAllWarehousesQuery : GetAllQuery<WarehouseListDTO>
    {
        public GetAllWarehousesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<WarehouseListDTO>>(warehouses);
        }
    }
}
