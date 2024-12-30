using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Warehouses.Stocks
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
