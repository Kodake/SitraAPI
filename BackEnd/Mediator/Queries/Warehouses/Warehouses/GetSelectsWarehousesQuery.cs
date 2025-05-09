using AutoMapper;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Warehouses
{
    public class GetSelectsWarehousesQuery : GetSelectsQuery<WarehouseListDTO>
    {
        public GetSelectsWarehousesQuery(StatusType status) : base(status) { }

        public override async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var warehouses = await unitOfWork.WarehouseRepository.GetAll(Status);
            return mapper.Map<List<WarehouseListDTO>>(warehouses);
        }
    }
}
