using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayables;

namespace SITRA.API.Endpoints
{
    public static class SpotPayablesEndpoints
    {
        public static RouteGroupBuilder MapSpotPayables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayables-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayables-get")).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<SpotPayableListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllSpotPayablesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<SpotPayableDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetSpotPayableByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }
    }
}

