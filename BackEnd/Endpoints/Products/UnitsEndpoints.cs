using BackEnd.DTOs;
using BackEnd.DTOs.Products.Units;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Products.Units;
using BackEnd.MediatR.Queries.Products.Units;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class UnitsEndpoints
    {
        public static RouteGroupBuilder MapUnits(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(3600)).Tag("units-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateUnitDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
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
            int id)
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
            int id,
            [FromBody] CreateUnitDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateUnitCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateUnitCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateUnitCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
