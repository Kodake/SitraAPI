using AutoMapper;
using BackEnd.DTOs.Products.Items;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class ItemsEndpoints
    {
        public static RouteGroupBuilder MapItems(this RouteGroupBuilder group)
        {
            group.MapGet("/pricing/{pricingId:int}/warehouse/{warehouseId:int}/", GetByPricingIdAndWarehouseId);
            return group;
        }

        static async Task<Ok<List<ItemListDTO>>> GetByPricingIdAndWarehouseId(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int pricingId, int warehouseId)
        {
            var items = await unitOfWork.ItemRepository.GetByPricingIdAndWarehouseId(pricingId, warehouseId);
            var itemsDTO = mapper.Map<List<ItemListDTO>>(items);
            itemsDTO.ForEach(dto => dto.PricingId = pricingId);

            return TypedResults.Ok(itemsDTO);
        }
    }
}