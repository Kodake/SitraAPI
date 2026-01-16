using AutoMapper;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetItemsByPricingIdAndWarehouseIdQuery(long pricingId, long warehouseId) : IQuery<List<ItemListDTO>>
    {
        public long PricingId { get; } = pricingId;
        public long WarehouseId { get; } = warehouseId;

        public async Task<List<ItemListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var items = await unitOfWork.ItemRepository.GetByPricingIdAndWarehouseId(PricingId, WarehouseId);
            var itemsDTO = mapper.Map<List<ItemListDTO>>(items);
            itemsDTO.ForEach(dto => dto.PricingId = PricingId);
            return itemsDTO;
        }
    }
}
