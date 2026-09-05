using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Warehouses.Stocks
{
    public class UpdateStockCommand : UpdateCommand<Stock, CreateStockDTO>
    {
        public UpdateStockCommand(Guid id, CreateStockDTO dto) : base(id, dto) { }

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
