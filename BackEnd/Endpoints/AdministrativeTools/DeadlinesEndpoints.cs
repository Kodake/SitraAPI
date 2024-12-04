using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class DeadlinesEndpoints
    {
        public static RouteGroupBuilder MapDeadlines(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("deadlines-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                //.RequireAuthorization("isAdmin")
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetAll(IDeadlineRepository repository,
            IMapper mapper, PaginationDTO pagination)
        {
            var deadlines = await repository.GetAll(pagination);
            var deadlinesDTO = mapper.Map<List<DeadlineListDTO>>(deadlines);
            return TypedResults.Ok(deadlinesDTO);
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetSelects(IDeadlineRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var deadlines = await repository.GetAll(status);
            var deadlinesDTO = mapper.Map<List<DeadlineListDTO>>(deadlines);
            return TypedResults.Ok(deadlinesDTO);
        }

        static async Task<Results<Ok<DeadlineDTO>, NotFound>> GetById(IDeadlineRepository repository,
            IMapper mapper, int id)
        {
            var deadline = await repository.GetById(id);

            if (deadline is null)
            {
                return TypedResults.NotFound();
            }

            var deadlineDTO = mapper.Map<DeadlineDTO>(deadline);
            return TypedResults.Ok(deadlineDTO);
        }

        static async Task<Results<Created<DeadlineDTO>, ValidationProblem>> Create([FromBody] CreateDeadlineDTO dto,
           IDeadlineRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var deadline = mapper.Map<Deadline>(dto);

            var id = await repository.Create(deadline);
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);
            var deadlineDTO = mapper.Map<DeadlineDTO>(deadline);
            return TypedResults.Created($"/deadlines/{id}", deadlineDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateDeadlineDTO dto,
           IDeadlineRepository repository,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var deadlineDB = await repository.GetById(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            deadlineDB.Name = dto.Name;
            deadlineDB.Days = dto.Days;
            deadlineDB.IsActive = deadlineDB.IsActive;

            await repository.Update(deadlineDB);
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             IDeadlineRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var deadlineDB = await repository.GetById(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             IDeadlineRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var deadlineDB = await repository.GetById(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);
            return TypedResults.Ok();
        }
    }
}
