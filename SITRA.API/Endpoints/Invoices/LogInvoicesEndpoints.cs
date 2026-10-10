using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Attributes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.LogInvoices;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Queries.Invoices.LogInvoices;

namespace SITRA.API.Endpoints.Invoices
{
    public static class LogInvoicesEndpoints
    {
        public static RouteGroupBuilder MapLogInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("logInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();

            group.MapGet("/{id:guid}/xml", GetXml).RequireAuthorization();

            group.MapPost("/{id:guid}/sync-dgii", SyncDgiiInvoice).RequireAuthorization();

            group.MapPost("/sync-pending", SyncPendingDgii);

            return group;
        }

        static async Task<IResult> SyncDgiiInvoice(
            Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new SITRA.Core.Mediator.Commands.Invoices.LogInvoices.SyncDgiiInvoiceStatusCommand(id);
            var result = await mediator.Send(command);
            if (!result.Success && (result.NotFound || result.Message == SITRA.Core.Helpers.Utilities.InvoiceNotFoundMessage))
            {
                return TypedResults.NotFound(result);
            }
            return TypedResults.Ok(result);
        }

        static async Task<IResult> SyncPendingDgii(
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] Microsoft.Extensions.Configuration.IConfiguration configuration,
            [FromServices] SITRA.Core.Interfaces.Services.AdministrativeTools.ICurrentTenantService currentTenantService)
        {
            string? apiKeyHeader = httpContext.Request.Headers["X-Sync-Api-Key"].FirstOrDefault();
            string? configuredApiKey = configuration["SyncJob:ApiKey"];

            bool isApiKeyAuthorized = !string.IsNullOrEmpty(configuredApiKey) &&
                                      string.Equals(apiKeyHeader, configuredApiKey, StringComparison.Ordinal);

            bool isUserAuthorized = httpContext.User?.Identity?.IsAuthenticated == true;

            if (!isApiKeyAuthorized && !isUserAuthorized)
            {
                return TypedResults.Unauthorized();
            }

            Guid? targetTenantId = isApiKeyAuthorized ? null : currentTenantService.TenantId;

            var command = new SITRA.Core.Mediator.Commands.Invoices.LogInvoices.SyncPendingDgiiInvoicesCommand(targetTenantId);
            var results = await mediator.Send(command);

            return TypedResults.Ok(results);
        }

        static async Task<Ok<List<LogInvoiceListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination,
            [FromQuery] DgiiStatus? status)
        {
            var query = new GetAllLogInvoicesQuery(pagination, status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<IResult> GetXml(
            Guid id,
            [FromServices] IMediator mediator)
        {
            var query = new GetLogInvoiceXmlQuery(id);
            var xml = await mediator.Send(query);
            
            if (string.IsNullOrEmpty(xml)) return TypedResults.NotFound();

            return TypedResults.Content(xml, "application/xml");
        }
    }
}