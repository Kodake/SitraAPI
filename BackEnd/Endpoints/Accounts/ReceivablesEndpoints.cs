using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Accounts;
using BackEnd.Helpers;
using BackEnd.Repositories.Accounts;
using Microsoft.AspNetCore.Http.HttpResults;

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
