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

namespace BackEnd.Endpoints.Products
{
    public static class ServicesEndpoints
    {
        public static RouteGroupBuilder MapServices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateServiceDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateServiceDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<ServiceListDTO>>> GetAll([FromServices] IServiceRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var services = await repository.GetAll(pagination);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);
            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Ok<List<ServiceListDTO>>> GetSelects(IServiceRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var services = await repository.GetAll(status);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);
            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Results<Ok<ServiceDTO>, NotFound>> GetById([FromServices] IServiceRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var service = await repository.GetById(id);

            if (service is null)
            {
                return TypedResults.NotFound();
            }

            var serviceDTO = mapper.Map<ServiceDTO>(service);
            return TypedResults.Ok(serviceDTO);
        }

        static async Task<Results<Created<ServiceDTO>, ValidationProblem>> Create([FromBody] CreateServiceDTO dto,
           [FromServices] IServiceRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var service = mapper.Map<Item>(dto);
            service.ItemType = ItemType.Service; // Create a Service

            var id = await repository.Create(service);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            var serviceDTO = mapper.Map<ServiceDTO>(service);
            return TypedResults.Created($"/services/{id}", serviceDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateServiceDTO dto,
            [FromServices] IServiceRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var serviceDB = await repository.GetById(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            serviceDB.Name = dto.Name;
            serviceDB.Reference = dto.Reference;
            serviceDB.Description = dto.Description;
            serviceDB.CategoryId = dto.CategoryId;
            serviceDB.UnitId = dto.UnitId;
            serviceDB.ItemTaxes = dto.TaxIds.Select(id => new ItemTaxes
            {
                TaxId = id,
                ItemId = serviceDB.Id
            }).ToList();

            await repository.Update(serviceDB);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IServiceRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await repository.GetById(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IServiceRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await repository.GetById(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }
    }
}
