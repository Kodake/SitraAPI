using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
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
            return group;
        }

        static async Task<Ok<List<ProductListDTO>>> GetAll([FromServices] IProductRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var products = await repository.GetAll(pagination);
            var productsDTO = mapper.Map<List<ProductListDTO>>(products);
            return TypedResults.Ok(productsDTO);
        }

        static async Task<Ok<List<ProductListDTO>>> GetSelects(IProductRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var products = await repository.GetAll(status);
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

        static async Task<Results<Created<ProductDTO>, ValidationProblem>> Create([FromBody] CreateProductDTO dto,
           [FromServices] IProductRepository repository,
           [FromServices] IPricingItemRepository pricingItemRepository,
           [FromServices] IPricingRepository pricingRepository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper,
           [FromServices] ApplicationDbContext context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var product = mapper.Map<Item>(dto);
                product.ItemType = ItemType.Product; // Create a Product

                var id = await repository.Create(product);
                var pricing = await pricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));

                if (pricing == null)
                {
                    pricing = new Pricing
                    {
                        Name = Enum.GetName(PricingName.Principal) ?? "Principal",
                        PricingType = PricingType.Value
                    };

                    pricing = await pricingRepository.Create(pricing);
                }

                var pricingItem = new PricingItem
                {
                    ItemId = id,
                    PricingId = pricing.Id,
                    InitialCost = dto.InitialCost,
                    BasePrice = dto.BasePrice,
                    Amount = dto.BasePrice,
                    IsPrincipal = true
                };

                await pricingItemRepository.Create(pricingItem);

                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("products-get", default);
                var productDTO = mapper.Map<ProductDTO>(product);
                return TypedResults.Created($"/products/{id}", productDTO);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while creating the product." } }
                });
            }
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateProductDTO dto,
        [FromServices] IProductRepository repository,
        IOutputCacheStore outputCacheStore,
        [FromServices] IMapper mapper)
        {
            var productDB = await repository.GetById(id);

            if (productDB is null)
            {
                return TypedResults.NotFound();
            }

            productDB.Name = dto.Name;
            productDB.Reference = dto.Reference;
            productDB.Description = dto.Description;
            productDB.InitialCost = dto.InitialCost;
            productDB.BasePrice = dto.BasePrice;
            productDB.BrandId = dto.BrandId;
            productDB.CategoryId = dto.CategoryId;
            productDB.UnitId = dto.UnitId;
            productDB.ItemTaxes = dto.TaxIds.Select(id => new ItemTaxes
            {
                TaxId = id,
                ItemId = productDB.Id
            }).ToList();

            await repository.Update(productDB);
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