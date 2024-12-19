using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.DTOs.Products;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using BackEnd.Repositories.AdministrativeTools;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class FiscalSupportsEndpoints
    {
        public static RouteGroupBuilder MapFiscalSupports(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscal-supports-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("fiscal-supports-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/supportType/{supportType}/warehouse/{warehouseId:int}", GetByDocumentTypeAndWarehouseId);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateFiscalSupportDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var fiscalSupports = await unitOfWork.FiscalSupportRepository.GetAllAsync(pagination);
            var fiscalSupportsDTO = mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);

            return TypedResults.Ok(fiscalSupportsDTO);
        }

        static async Task<Ok<List<FiscalSupportListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var fiscalSupports = await unitOfWork.FiscalSupportRepository.GetAll(status);
            var fiscalSupportsDTO = mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);

            return TypedResults.Ok(fiscalSupportsDTO);
        }

        static async Task<Results<Ok<FiscalSupportDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id);

            if (fiscalSupport is null)
            {
                return TypedResults.NotFound();
            }

            var fiscalSupportDTO = mapper.Map<FiscalSupportDTO>(fiscalSupport);

            return TypedResults.Ok(fiscalSupportDTO);
        }

        static async Task<Ok<List<FiscalSupportDTO>>> GetByDocumentTypeAndWarehouseId(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, [FromRoute] FiscalSupportType supportType, int warehouseId)
        {
            var fiscalSupport = await unitOfWork.FiscalSupportRepository.GetByDocumentTypeAndWarehouseId(supportType, warehouseId);
            var fiscalSupportDTO = mapper.Map<List<FiscalSupportDTO>>(fiscalSupport);

            return TypedResults.Ok(fiscalSupportDTO);
        }

        static async Task<Results<Created<FiscalSupportDTO>, ValidationProblem>> Create([FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var fiscalSupport = mapper.Map<FiscalSupport>(dto);

            await unitOfWork.FiscalSupportRepository.CreateAsync(fiscalSupport);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);

            var fiscalSupportDTO = mapper.Map<FiscalSupportDTO>(fiscalSupport);

            return TypedResults.Created($"/fiscalSupports/{fiscalSupport.Id}", fiscalSupportDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateFiscalSupportDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            fiscalSupportDB.DocumentType = dto.DocumentType;
            fiscalSupportDB.FiscalSupportType = dto.FiscalSupportType;
            fiscalSupportDB.Name = dto.Name;
            fiscalSupportDB.SpaceNumber = dto.SpaceNumber;
            fiscalSupportDB.Symbol = dto.Symbol;
            fiscalSupportDB.WarehouseId = dto.WarehouseId;
            fiscalSupportDB.InitialSequence = dto.InitialSequence;
            fiscalSupportDB.FinalSequence = dto.FinalSequence;
            fiscalSupportDB.IsActive = fiscalSupportDB.IsActive;

            unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.FiscalSupportRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(id);

            if (fiscalSupportDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.FiscalSupportRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscal-supports-get", default);

            return TypedResults.Ok();
        }
    }
}
