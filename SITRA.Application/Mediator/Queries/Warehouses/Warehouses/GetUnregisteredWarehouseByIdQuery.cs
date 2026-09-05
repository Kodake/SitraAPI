using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
{
    public class GetUnregisteredWarehouseByIdQuery(Guid id) : IQuery<List<WarehouseListDTO>>
    {
        public Guid Id { get; } = id;

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
