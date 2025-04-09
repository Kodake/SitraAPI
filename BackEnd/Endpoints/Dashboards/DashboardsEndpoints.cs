using BackEnd.DTOs;
using BackEnd.MediatR.Queries.Dashboards;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Endpoints
{
    public static class DashboardsEndpoints
    {
        public static RouteGroupBuilder MapDashboards(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll);
                //.CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("dashboards-get"))
                //.AddPaginationParamsOpenAPI();
            return group;
        }

        static async Task<Ok<List<DashboardListDTO>>> GetAll(
            [FromServices] IMediator mediator)
        {
            var query = new GetAllDashboardsQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}