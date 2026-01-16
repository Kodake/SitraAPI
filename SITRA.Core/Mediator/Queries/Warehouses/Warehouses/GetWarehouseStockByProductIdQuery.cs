using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Warehouses
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
