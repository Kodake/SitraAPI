using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Warehouses
{
    public class GetAllWarehousesQuery : GetAllQuery<WarehouseListDTO>
    {
        public GetAllWarehousesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetAllAsync(Pagination);
            return mapper.Map<List<WarehouseListDTO>>(warehouses);
        }
    }
}
