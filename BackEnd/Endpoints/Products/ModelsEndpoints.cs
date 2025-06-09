using BackEnd.DTOs;
using BackEnd.DTOs.Products.Models;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Products.Models;
using BackEnd.Mediator.Queries.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class ModelsEndpoints
    {
        public static RouteGroupBuilder MapModels(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("models-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("models-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateModelDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateModelDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);
            group.MapDelete("/{id:long}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<ModelListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllModelsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ModelListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsModelsQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ModelDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetModelByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateModelDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateModelCommand(dto);
            var categoryDTO = await mediator.Send(command);
            return categoryDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateModelDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateModelCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateModelCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateModelCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
