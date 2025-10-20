using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Warehouses.Stocks
{
    public class GetAllStocksByWarehouseIdsQuery : GetAllQuery<StockListDTO>
    {
        public List<long> WarehouseIds { get; }
        public PaginationDTO PaginationDTO { get; }
        public GetAllStocksByWarehouseIdsQuery(List<long> warehouseIds, PaginationDTO pagination) : base(pagination) 
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
