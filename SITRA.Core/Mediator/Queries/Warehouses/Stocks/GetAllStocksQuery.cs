using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Stocks
{
    public class GetAllStocksQuery : GetAllQuery<StockListDTO>
    {
        public GetAllStocksQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<StockListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var stocks = await unitOfWork.StockRepository.GetAllAsync(Pagination);
            return mapper.Map<List<StockListDTO>>(stocks);
        }
    }
}
