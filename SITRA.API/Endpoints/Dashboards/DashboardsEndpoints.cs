using Mediator;
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
            group.MapGet("/stock-analysis", GetStockAnalysis).RequireAuthorization();
            group.MapGet("/overall-report", GetOverallReport).RequireAuthorization();
            group.MapGet("/cash-flow", GetCashFlowReport).RequireAuthorization();
            group.MapGet("/top-sales", GetTopSales).RequireAuthorization();
            group.MapGet("/top-customers", GetTopCustomers).RequireAuthorization();
            return group;
        }

        static async Task<Results<Ok<DashboardDTO>, NotFound>> GetAll(
            [FromServices] IMediator mediator)
        {
            var query = new GetAllDashboardsQuery();
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Ok<StockAnalysisDTO>> GetStockAnalysis([FromQuery] int? year, [FromServices] IMediator mediator)
        {
            var query = new GetStockAnalysisQuery { Year = year ?? DateTime.UtcNow.Year };
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<OverallReportDTO>> GetOverallReport([FromServices] IMediator mediator)
        {
            var query = new GetOverallReportQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<CashFlowReportDTO>> GetCashFlowReport([FromQuery] int? year, [FromServices] IMediator mediator)
        {
            var query = new GetCashFlowReportQuery { Year = year ?? DateTime.UtcNow.Year };
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<TopSaleDTO>>> GetTopSales([FromQuery] int? month, [FromQuery] int? year, [FromServices] IMediator mediator)
        {
            var query = new GetTopSalesQuery { 
                Month = month ?? DateTime.UtcNow.Month, 
                Year = year ?? DateTime.UtcNow.Year 
            };
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<TopCustomerDTO>>> GetTopCustomers([FromQuery] int? month, [FromQuery] int? year, [FromServices] IMediator mediator)
        {
            var query = new GetTopCustomersQuery { 
                Month = month ?? DateTime.UtcNow.Month, 
                Year = year ?? DateTime.UtcNow.Year 
            };
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}

