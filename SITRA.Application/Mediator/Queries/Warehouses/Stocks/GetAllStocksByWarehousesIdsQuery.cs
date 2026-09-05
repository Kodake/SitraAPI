using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Stocks
{
    public class GetAllStocksByWarehouseIdsQuery : GetAllQuery<StockListDTO>
    {
        public List<Guid> WarehouseIds { get; }
        public PaginationDTO PaginationDTO { get; }
        public GetAllStocksByWarehouseIdsQuery(List<Guid> warehouseIds, PaginationDTO pagination) : base(pagination)
        {
            WarehouseIds = warehouseIds;
            PaginationDTO = pagination;
        }

        public override async Task<List<StockListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var stocks = await unitOfWork.StockRepository.GetAllByWarehouseIdsAsync(WarehouseIds, PaginationDTO);
            return mapper.Map<List<StockListDTO>>(stocks);
        }
    }
}
