using AutoMapper;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Entities.Warehouses;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Warehouses.Stocks
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
