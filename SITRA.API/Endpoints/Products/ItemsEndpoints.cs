using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.Mediator.Queries.Products.Items;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SITRA.API.Endpoints
{
    public static class ItemsEndpoints
    {
        public static RouteGroupBuilder MapItems(this RouteGroupBuilder group)
        {
            group.MapGet("/pricing/{pricingId:long}/warehouse/{warehouseId:long}/", GetByPricingIdAndWarehouseId).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ItemListDTO>>> GetByPricingIdAndWarehouseId(
            [FromServices] IMediator mediator,
            long pricingId, long warehouseId)
        {
            var query = new GetItemsByPricingIdAndWarehouseIdQuery(pricingId, warehouseId);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}

