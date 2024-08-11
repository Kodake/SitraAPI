using AutoMapper;
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
    public static class BrandsEndpoints
    {
        public static RouteGroupBuilder MapBrands(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("brands-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateBrandDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<BrandListDTO>>> GetAll(IBrandRepository repository,
            IMapper mapper, PaginationDTO paginacion)
        {
            var brands = await repository.GetAll(paginacion);
            var brandsDTO = mapper.Map<List<BrandListDTO>>(brands);
            return TypedResults.Ok(brandsDTO);
        }

        static async Task<Ok<List<BrandListDTO>>> GetSelects(IBrandRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var brands = await repository.GetAll(status);
            var brandsDTO = mapper.Map<List<BrandListDTO>>(brands);
            return TypedResults.Ok(brandsDTO);
        }

        static async Task<Results<Ok<BrandDTO>, NotFound>> GetById(IBrandRepository repository,
            IMapper mapper, int id)
        {
            var brand = await repository.GetById(id);

            if (brand is null)
            {
                return TypedResults.NotFound();
            }

            var brandDTO = mapper.Map<BrandDTO>(brand);
            return TypedResults.Ok(brandDTO);
        }

        static async Task<Results<Created<BrandDTO>, ValidationProblem>> Create([FromForm] CreateBrandDTO createBrandDTO,
           IBrandRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var brand = mapper.Map<Brand>(createBrandDTO);

            var id = await repository.Create(brand);
            await outputCacheStore.EvictByTagAsync("brands-get", default);
            var brandDTO = mapper.Map<BrandDTO>(brand);
            return TypedResults.Created($"/brands/{id}", brandDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromForm] CreateBrandDTO createBrandDTO,
           IBrandRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var existingBrand = await repository.GetById(id);

            if (existingBrand is null)
            {
                return TypedResults.NotFound();
            }

            existingBrand.Name = createBrandDTO.Name;
            existingBrand.Name = createBrandDTO.Name;
            existingBrand.IsActive = existingBrand.IsActive;

            await repository.Update(existingBrand);
            await outputCacheStore.EvictByTagAsync("brands-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             IBrandRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var brandDB = await repository.GetById(id);

            if (brandDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("brands-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             IBrandRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var brandDB = await repository.GetById(id);

            if (brandDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("brands-get", default);
            return TypedResults.Ok();
        }
    }
}
