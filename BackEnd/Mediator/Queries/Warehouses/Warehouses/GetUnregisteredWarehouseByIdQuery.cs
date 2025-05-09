using AutoMapper;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Warehouses
{
    public class GetUnregisteredWarehouseByIdQuery(long id) : IQuery<List<WarehouseListDTO>>
    {
        public long Id { get; } = id;

        public async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var unregisteredWarehouses = await unitOfWork.WarehouseRepository.GetUnregisteredWarehouseById(Id);
            var unregisteredWarehousesDTO = mapper.Map<List<WarehouseListDTO>>(unregisteredWarehouses);
            return unregisteredWarehousesDTO;
        }
    }
}
