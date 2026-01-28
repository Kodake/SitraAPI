using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Warehouses.Stocks
{
    public class GetStockByIdQuery : GetByIdQuery<StockDTO>
    {
        public GetStockByIdQuery(Guid id) : base(id) { }

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
