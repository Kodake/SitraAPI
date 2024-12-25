using BackEnd.DTOs;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Products.Brands;
using BackEnd.MediatR.Queries.Products.Brands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class BrandsEndpoints
    {
        public static RouteGroupBuilder MapBrands(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:long}/activate", Activate);
            group.MapDelete("/{id:long}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<BrandListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllBrandsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<BrandListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsBrandsQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<BrandDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            int id)
        {
            var query = new GetBrandByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateBrandDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateBrandCommand(dto);
            var brandDTO = await mediator.Send(command);
            return brandDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            int id,
            [FromBody] CreateBrandDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateBrandCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateBrandCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateBrandCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}
