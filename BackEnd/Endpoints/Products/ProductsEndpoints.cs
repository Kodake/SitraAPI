using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
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
            return group;
        }

        static async Task<Ok<List<ProductListDTO>>> GetAll([FromServices] IProductRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var products = await repository.GetAll(pagination);
            var productsDTO = mapper.Map<List<ProductListDTO>>(products);
            return TypedResults.Ok(productsDTO);
        }

        static async Task<Results<Ok<ProductDTO>, NotFound>> GetById([FromServices] IProductRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var product = await repository.GetById(id);

            if (product is null)
            {
                return TypedResults.NotFound();
            }

            var productDTO = mapper.Map<ProductDTO>(product);
            return TypedResults.Ok(productDTO);
        }

        static async Task<Results<Created<ProductDTO>, ValidationProblem>> Create([FromBody] CreateProductDTO createProductDTO,
           [FromServices] IProductRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var product = mapper.Map<Product>(createProductDTO);

            var id = await repository.Create(product);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            var productDTO = mapper.Map<ProductDTO>(product);
            return TypedResults.Created($"/products/{id}", productDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateProductDTO createProductDTO,
            [FromServices] IProductRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var existingProduct = await repository.GetById(id);

            if (existingProduct is null)
            {
                return TypedResults.NotFound();
            }

            existingProduct.Name = createProductDTO.Name;
            existingProduct.Reference = createProductDTO.Reference;
            existingProduct.Description = createProductDTO.Description;
            existingProduct.InitialCost = createProductDTO.InitialCost;
            existingProduct.BasePrice = createProductDTO.BasePrice;
            existingProduct.BrandId = createProductDTO.BrandId;
            existingProduct.CategoryId = createProductDTO.CategoryId;
            existingProduct.UnitId = createProductDTO.UnitId;
            existingProduct.ProductTaxes = createProductDTO.TaxIds.Select(id => new ProductTaxes
            {
                TaxId = id,
                ProductId = existingProduct.Id
            }).ToList();

            await repository.Update(existingProduct);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IProductRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var productDB = await repository.GetById(id);

            if (productDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IProductRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var productDB = await repository.GetById(id);

            if (productDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("products-get", default);
            return TypedResults.Ok();
        }
    }
}
