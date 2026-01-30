using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Products.Brands;
using SITRA.Core.Mediator.Queries.Products.Brands;

namespace SITRA.API.Endpoints
{
    public static class BrandsEndpoints
    {
        public static RouteGroupBuilder MapBrands(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
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
            Guid id)
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
            Guid id,
            [FromBody] CreateBrandDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateBrandCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateBrandCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateBrandCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

