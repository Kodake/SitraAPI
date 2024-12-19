using AutoMapper;
using BackEnd.DTOs;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using BackEnd.Repositories;
using BackEnd.DTOs.AdministrativeTools.Deadlines;

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
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateDeadlineDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            return group;
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetAll(IUnitOfWork unitOfWork,
            IMapper mapper, PaginationDTO pagination)
        {
            var deadlines = await unitOfWork.DeadlineRepository.GetAllAsync(pagination);
            var deadlinesDTO = mapper.Map<List<DeadlineListDTO>>(deadlines);

            return TypedResults.Ok(deadlinesDTO);
        }

        static async Task<Ok<List<DeadlineListDTO>>> GetSelects(IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var deadlines = await unitOfWork.DeadlineRepository.GetAll(status);
            var deadlinesDTO = mapper.Map<List<DeadlineListDTO>>(deadlines);

            return TypedResults.Ok(deadlinesDTO);
        }

        static async Task<Results<Ok<DeadlineDTO>, NotFound>> GetById(IUnitOfWork unitOfWork,
            IMapper mapper, int id)
        {
            var deadline = await unitOfWork.DeadlineRepository.GetByIdAsync(id);

            if (deadline is null)
            {
                return TypedResults.NotFound();
            }

            var deadlineDTO = mapper.Map<DeadlineDTO>(deadline);

            return TypedResults.Ok(deadlineDTO);
        }

        static async Task<Results<Created<DeadlineDTO>, ValidationProblem>> Create([FromBody] CreateDeadlineDTO dto,
           IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var deadline = mapper.Map<Deadline>(dto);

            await unitOfWork.DeadlineRepository.CreateAsync(deadline);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);

            var deadlineDTO = mapper.Map<DeadlineDTO>(deadline);
            return TypedResults.Created($"/deadlines/{deadline.Id}", deadlineDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateDeadlineDTO dto,
           IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           IMapper mapper)
        {
            var deadlineDB = await unitOfWork.DeadlineRepository.GetByIdAsync(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            deadlineDB.Name = dto.Name;
            deadlineDB.Days = dto.Days;

            unitOfWork.DeadlineRepository.Update(deadlineDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var deadlineDB = await unitOfWork.DeadlineRepository.GetByIdAsync(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.DeadlineRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var deadlineDB = await unitOfWork.DeadlineRepository.GetByIdAsync(id);

            if (deadlineDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.DeadlineRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("deadlines-get", default);

            return TypedResults.Ok();
        }
    }
}
