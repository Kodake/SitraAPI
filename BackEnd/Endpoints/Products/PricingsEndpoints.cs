using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Pricings;
using BackEnd.Entities.Products;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class PricingsEndpoints
    {
        public static RouteGroupBuilder MapPricings(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/unregistered-pricings/{id:int}", GetUnregisteredPricingsForItem).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pricings-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/selects", GetPricings);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreatePricingDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<PricingListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var pricings = await unitOfWork.PricingRepository.GetAllAsync(pagination);
            var pricingsDTO = mapper.Map<List<PricingListDTO>>(pricings);

            return TypedResults.Ok(pricingsDTO);
        }

        static async Task<Ok<List<PricingListDTO>>> GetUnregisteredPricingsForItem(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var unregPricings = await unitOfWork.PricingRepository.GetUnregisteredPricingsForItem(id);
            var unregPricingsDTO = mapper.Map<List<PricingListDTO>>(unregPricings);

            return TypedResults.Ok(unregPricingsDTO);
        }

        static async Task<Results<Ok<PricingDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var pricing = await unitOfWork.PricingRepository.GetByIdAsync(id);

            if (pricing is null)
            {
                return TypedResults.NotFound();
            }

            var pricingDTO = mapper.Map<PricingDTO>(pricing);

            return TypedResults.Ok(pricingDTO);
        }

        static async Task<Ok<List<PricingListDTO>>> GetPricings(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            var pricings = await unitOfWork.PricingRepository.GetAll();

            var pricingsDTO = mapper.Map<List<PricingListDTO>>(pricings);

            return TypedResults.Ok(pricingsDTO);
        }

        static async Task<Results<Created<PricingDTO>, ValidationProblem>> Create([FromBody] CreatePricingDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var pricing = mapper.Map<Pricing>(dto);

            await unitOfWork.PricingRepository.CreateAsync(pricing);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricings-get", default);

            var pricingDTO = mapper.Map<PricingDTO>(pricing);

            return TypedResults.Created($"/pricings/{pricing.Id}", pricingDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreatePricingDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var pricingDB = await unitOfWork.PricingRepository.GetByIdAsync(id);

            if (pricingDB is null)
            {
                return TypedResults.NotFound();
            }

            pricingDB.Name = dto.Name;
            pricingDB.PricingType = dto.PricingType;
            pricingDB.Percentage = dto.Percentage;

            unitOfWork.PricingRepository.Update(pricingDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("pricings-get", default);

            return TypedResults.Ok();
        }
    }
}
