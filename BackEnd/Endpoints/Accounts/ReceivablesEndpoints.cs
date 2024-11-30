using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts;
using BackEnd.DTOs.AdministrativeTools;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Accounts;
using BackEnd.Repositories.AdministrativeTools;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading.Tasks.Dataflow;

namespace BackEnd.Endpoints
{
    public static class ReceivablesEndpoints
    {
        public static RouteGroupBuilder MapReceivables(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("receivables-get"))
                .AddPaginationParamsOpenAPI();
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<ReceivableListDTO>>> GetAll(IReceivableRepository repository,
            IMapper mapper, PaginationDTO paginacion)
        {
            var receivables = await repository.GetAll(paginacion);
            var receivablesDTO = mapper.Map<List<ReceivableListDTO>>(receivables);
            return TypedResults.Ok(receivablesDTO);
        }
    }
}
