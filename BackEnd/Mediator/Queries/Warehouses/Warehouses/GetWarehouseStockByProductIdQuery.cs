using AutoMapper;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Warehouses
{
    public class GetWarehouseStockByProductIdQuery(long productId) : IQuery<List<WarehouseListDTO>>
    {
        public long ProductId { get; } = productId;

        public async Task<List<WarehouseListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var stockWarehouses = await unitOfWork.WarehouseRepository.GetStockWarehouseByProductId(ProductId);
            var stockWarehousesDTO = mapper.Map<List<WarehouseListDTO>>(stockWarehouses);
            return stockWarehousesDTO;
        }
    }
}
