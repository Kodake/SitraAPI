using MediatR;
using AutoMapper;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Items
{
    public class GetItemsByPricingIdAndWarehouseIdQuery : IRequest<List<ItemListDTO>>
    {
        public Guid PricingId { get; set; }
        public Guid WarehouseId { get; set; }
        public string? FiscalSupportType { get; set; }

        public GetItemsByPricingIdAndWarehouseIdQuery(Guid pricingId, Guid warehouseId, string? fiscalSupportType = null)
        {
            PricingId = pricingId;
            WarehouseId = warehouseId;
            FiscalSupportType = fiscalSupportType;
        }
    }

    public class GetItemsByPricingIdAndWarehouseIdQueryHandler : IRequestHandler<GetItemsByPricingIdAndWarehouseIdQuery, List<ItemListDTO>>
    {
        private readonly SITRA.Core.Interfaces.Services.Products.IItemService _itemService;
        private readonly IMapper _mapper;

        public GetItemsByPricingIdAndWarehouseIdQueryHandler(SITRA.Core.Interfaces.Services.Products.IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }

        public async Task<List<ItemListDTO>> Handle(GetItemsByPricingIdAndWarehouseIdQuery request, CancellationToken cancellationToken)
        {
            var items = await _itemService.GetValidItemsForPricingAndWarehouseAsync(request.PricingId, request.WarehouseId, request.FiscalSupportType, cancellationToken);
            var itemsDTO = _mapper.Map<List<ItemListDTO>>(items);
            itemsDTO.ForEach(dto => dto.PricingId = request.PricingId);
            return itemsDTO;
        }
    }
}
