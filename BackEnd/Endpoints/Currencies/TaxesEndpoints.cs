using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Currencies;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Currencies;
using BackEnd.Repositories.Products;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class TaxesEndpoints
    {
        public static RouteGroupBuilder MapTaxes(this RouteGroupBuilder group)
        {
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("taxes-get"));
            return group;
        }

        static async Task<Ok<List<TaxListDTO>>> GetSelects(ITaxRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var taxes = await repository.GetAll(status);
            var taxesDTO = mapper.Map<List<TaxListDTO>>(taxes);
            return TypedResults.Ok(taxesDTO);
        }
    }
}
