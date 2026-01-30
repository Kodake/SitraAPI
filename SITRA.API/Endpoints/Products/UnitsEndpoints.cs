using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Products.Units;
using SITRA.Core.Mediator.Queries.Products.Units;

namespace SITRA.API.Endpoints
{
    public static class UnitsEndpoints
    {
        public static RouteGroupBuilder MapUnits(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<UnitListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllUnitsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<UnitListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsUnitsQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<UnitDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetUnitByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateUnitDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateUnitCommand(dto);
            var unitDTO = await mediator.Send(command);
            return unitDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            Guid id,
            [FromBody] CreateUnitDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateUnitCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateUnitCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateUnitCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

