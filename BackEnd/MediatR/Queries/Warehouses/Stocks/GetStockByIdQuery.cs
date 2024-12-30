using AutoMapper;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Warehouses.Stocks
{
    public class GetStockByIdQuery : GetByIdQuery<StockDTO>
    {
        public GetStockByIdQuery(int id) : base(id) { }

        public override async Task<StockDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var stock = await unitOfWork.StockRepository.GetByIdAsync(Id);
            return mapper.Map<StockDTO>(stock);
        }
    }
}