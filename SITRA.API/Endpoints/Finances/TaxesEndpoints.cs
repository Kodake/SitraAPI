using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Enums;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Finances.Taxes;
using SITRA.Core.Mediator.Queries.Finances.Taxes;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SITRA.API.Endpoints
{
    public static class TaxesEndpoints
    {
        public static RouteGroupBuilder MapTaxes(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("taxes-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("taxes-get")).RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateTaxDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateTaxDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<TaxListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllTaxesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<TaxListDTO>>> GetSelects(
            [FromServices] IMediator mediator, [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsTaxesQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<TaxDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetTaxByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateTaxDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateTaxCommand(dto);
            var taxDTO = await mediator.Send(command);
            return taxDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateTaxDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateTaxCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateTaxCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateTaxCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

