using AutoMapper;
using BackEnd.DTOs.Finances;
using BackEnd.DTOs.Products;
using BackEnd.DTOs.Warehouses;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackEnd.Endpoints
{
    public static class ItemsEndpoints
    {
        public static RouteGroupBuilder MapItems(this RouteGroupBuilder group)
        {
            group.MapGet("/pricing/{pricingId:int}/warehouse/{warehouseId:int}/", GetByPricingIdAndWarehouseId);
            return group;
        }

        static async Task<Ok<List<ItemListDTO>>> GetByPricingIdAndWarehouseId(IItemRepository repository,
            IMapper mapper, int pricingId, int warehouseId)
        {
            var items = await repository.GetByPricingIdAndWarehouseId(pricingId, warehouseId);
            var itemsDTO = mapper.Map<List<ItemListDTO>>(items);
            itemsDTO.ForEach(dto => dto.PricingId = pricingId);

            return TypedResults.Ok(itemsDTO);
        }
    }
}