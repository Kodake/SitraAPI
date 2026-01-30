using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.Mediator.Queries.Dashboards;

namespace SITRA.API.Endpoints
{
    public static class DashboardsEndpoints
    {
        public static RouteGroupBuilder MapDashboards(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).RequireAuthorization();
            //.CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("dashboards-get"))
            //.AddPaginationParamsOpenAPI();
            return group;
        }

        static async Task<Results<Ok<DashboardDTO>, NotFound>> GetAll(
            [FromServices] IMediator mediator)
        {
            var query = new GetAllDashboardsQuery();
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }
    }
}

