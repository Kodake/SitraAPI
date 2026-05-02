using AutoMapper;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetItemsByPricingIdAndWarehouseIdQuery(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null) : IQuery<List<ItemListDTO>>
    {
        public Guid PricingId { get; } = pricingId;
        public Guid WarehouseId { get; } = warehouseId;
        public string? FiscalSupportType { get; } = fiscalSupportType;

        public async Task<List<ItemListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var items = await unitOfWork.ItemRepository.GetByPricingIdAndWarehouseId(PricingId, WarehouseId, FiscalSupportType, cancellationToken);
            var itemsDTO = mapper.Map<List<ItemListDTO>>(items);
            itemsDTO.ForEach(dto => dto.PricingId = PricingId);
            return itemsDTO;
        }
    }
}
