using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayments;

namespace SITRA.API.Endpoints
{
    public static class SpotPaymentsEndpoints
    {
        public static RouteGroupBuilder MapSpotPayments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayments-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayments-get")).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<SpotPaymentListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllSpotPaymentsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<SpotPaymentDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetSpotPaymentByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }
    }
}

