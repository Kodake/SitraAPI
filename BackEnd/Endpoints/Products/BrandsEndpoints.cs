using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
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

        static async Task<Ok<List<BrandListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var brands = await unitOfWork.BrandRepository.GetAllAsync(pagination);
            var brandsDTO = mapper.Map<List<BrandListDTO>>(brands);

            return TypedResults.Ok(brandsDTO);
        }

        static async Task<Ok<List<BrandListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var brands = await unitOfWork.BrandRepository.GetAll(status);
            var brandsDTO = mapper.Map<List<BrandListDTO>>(brands);

            return TypedResults.Ok(brandsDTO);
        }

        static async Task<Results<Ok<BrandDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var brand = await unitOfWork.BrandRepository.GetByIdAsync(id);

            if (brand is null)
            {
                return TypedResults.NotFound();
            }

            var brandDTO = mapper.Map<BrandDTO>(brand);

            return TypedResults.Ok(brandDTO);
        }

        static async Task<Results<Created<BrandDTO>, ValidationProblem>> Create([FromBody] CreateBrandDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var brand = mapper.Map<Brand>(dto);

            await unitOfWork.BrandRepository.CreateAsync(brand);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            var brandDTO = mapper.Map<BrandDTO>(brand);

            return TypedResults.Created($"/brands/{brand.Id}", brandDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateBrandDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var brandDB = await unitOfWork.BrandRepository.GetByIdAsync(id);

            if (brandDB is null)
            {
                return TypedResults.NotFound();
            }

            brandDB.Name = dto.Name;
            brandDB.IsActive = brandDB.IsActive;

            unitOfWork.BrandRepository.Update(brandDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var brandDB = await unitOfWork.BrandRepository.GetByIdAsync(id);

            if (brandDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.BrandRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var brandDB = await unitOfWork.BrandRepository.GetByIdAsync(id);

            if (brandDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.BrandRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            return TypedResults.Ok();
        }
    }
}
