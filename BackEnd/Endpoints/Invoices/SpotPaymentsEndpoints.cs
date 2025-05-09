using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SpotPayments;
using BackEnd.Helpers;
using BackEnd.Mediator.Queries.Invoices.SpotPayments;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class SpotPaymentsEndpoints
    {
        public static RouteGroupBuilder MapSpotPayments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayments-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetById).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("spotPayments-get"));
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
            long id)
        {
            var query = new GetSpotPaymentByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }
    }
}
