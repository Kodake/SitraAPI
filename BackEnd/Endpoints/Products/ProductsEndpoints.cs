using System.Globalization;
using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Products;
using BackEnd.DTOs.Products.Services;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands;
using BackEnd.MediatR.Commands.Products.Products;
using BackEnd.MediatR.Queries.Products.Products;
using BackEnd.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class ProductsEndpoints
    {
        public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/stock-products", GetStockProducts);
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateProductDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateProductDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            group.MapPost("/upload-csv", UploadCsv).DisableAntiforgery()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<ProductListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllProductsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ProductListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsProductsQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ProductListDTO>>> GetStockProducts(
            [FromServices] IMediator mediator)
        {
            var query = new GetStockProductsQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ProductDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            int id)
        {
            var query = new GetProductByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateProductDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateProductCommand(dto);
            var productDTO = await mediator.Send(command);
            return productDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            int id,
            [FromBody] CreateProductDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateProductCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateProductCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] int id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateProductCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        public static async Task<Results<Ok, ValidationProblem>> UploadCsv(
            [FromForm] CsvDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UploadCsvProductCommand(dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { "An error occurred while processing the CSV." } }
            });
        }
    }
}