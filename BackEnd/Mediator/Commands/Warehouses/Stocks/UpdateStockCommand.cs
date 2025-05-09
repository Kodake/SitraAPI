using AutoMapper;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Warehouses.Stocks
{
    public class UpdateStockCommand : UpdateCommand<Stock, CreateStockDTO>
    {
        public UpdateStockCommand(long id, CreateStockDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var stockDB = await unitOfWork.StockRepository.GetByIdAsync(Id);

            if (stockDB == null) return false;

            mapper.Map(Dto, stockDB);
            unitOfWork.StockRepository.Update(stockDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);

            return true;
        }
    }
}
