using BackEnd.DTOs;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Products.SubCategories;
using BackEnd.Mediator.Queries.Products.SubCategories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class SubCategoriesEndpoints
    {
        public static RouteGroupBuilder MapSubCategories(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("subCategories-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("subCategories-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateSubCategoryDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateSubCategoryDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);
            group.MapDelete("/{id:long}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<SubCategoryListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllSubCategoriesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<SubCategoryListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsSubCategoriesQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<SubCategoryDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetSubCategoryByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateSubCategoryDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateSubCategoryCommand(dto);
            var categoryDTO = await mediator.Send(command);
            return categoryDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateSubCategoryDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateSubCategoryCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateSubCategoryCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateSubCategoryCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
