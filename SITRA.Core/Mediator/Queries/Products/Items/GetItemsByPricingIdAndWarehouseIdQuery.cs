using AutoMapper;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetItemsByPricingIdAndWarehouseIdQuery(Guid pricingId, Guid warehouseId) : IQuery<List<ItemListDTO>>
    {
        public Guid PricingId { get; } = pricingId;
        public Guid WarehouseId { get; } = warehouseId;

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
