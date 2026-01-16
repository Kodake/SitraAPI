using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Warehouses.Stocks
{
    public class CreateStockCommand : CreateCommand<Stock, CreateStockDTO>
    {
        public CreateStockCommand(CreateStockDTO dto) : base(dto) { }

        public override async Task<CreateStockDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var stock = await unitOfWork.StockRepository.GetByItemIdAndWarehouseId(Dto.ItemId, Dto.WarehouseId);

            if (stock != null)
            {
                stock.Qty += Dto.Qty;
                unitOfWork.StockRepository.Update(stock);
            }
            else
            {
                stock = mapper.Map<Stock>(Dto);
                await unitOfWork.StockRepository.CreateAsync(stock);
            }

            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("stocks-get", cancellationToken);

            return Dto;
        }
    }
}
