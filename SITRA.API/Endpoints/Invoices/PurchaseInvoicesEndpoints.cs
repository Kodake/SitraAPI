using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Attributes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices;

namespace SITRA.API.Endpoints.Invoices
{
    public static class PurchaseInvoicesEndpoints
    {
        public static RouteGroupBuilder MapPurchaseInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/purchases-range", GetAllByDateRange).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/purchases-with-balance", GetAllWithBalance).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/purchases-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreatePurchaseInvoiceDTO>>().WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/download", DownloadPdf).WithOpenApi().RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<PurchaseInvoiceListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllPurchaseInvoicesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<PurchaseInvoiceDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetPurchaseInvoiceByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        [Idempotent]
        static async Task<Results<Created<CreatePurchaseInvoiceDTO>, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePurchaseInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreatePurchaseInvoiceCommand(dto);
            var purchaseDTO = await mediator.Send(command);
            return purchaseDTO == null
                ? TypedResults.NotFound()
                : TypedResults.Created(string.Empty, purchaseDTO);
        }

        static async Task<IResult> GeneratePdf(
            [FromRoute] Guid invoiceId,
            [FromServices] IMediator mediator,
            [FromServices] Microsoft.Extensions.Options.IOptions<SITRA.Core.Models.Dgii.DgiiSettings> dgiiSettings,
            [FromServices] SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService dgiiStorageService)
        {
            try
            {
                var query = new GetPurchaseInvoicePdfQuery(invoiceId, dgiiSettings.Value, dgiiStorageService);
                var result = await mediator.Send(query);

                if (result == null)
                    return Results.NotFound();

                return Results.File(
                    result.Value.FileBytes,
                    "application/pdf",
                    result.Value.FileName);
            }
            catch (OperationCanceledException)
            {
                return Results.Empty;
            }
        }

        static async Task<Results<FileContentHttpResult, NotFound>> DownloadPdf(
            [FromRoute] Guid invoiceId,
            [FromServices] IMediator mediator,
            [FromServices] Microsoft.Extensions.Options.IOptions<SITRA.Core.Models.Dgii.DgiiSettings> dgiiSettings,
            [FromServices] SITRA.Core.Interfaces.Services.Storage.IDgiiStorageService dgiiStorageService)
        {
            var query = new DownloadPurchaseInvoicePdfQuery(invoiceId, dgiiSettings.Value, dgiiStorageService);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }


        static async Task<Results<FileContentHttpResult, NotFound>> GenerateReportPdf(
            [FromServices] IMediator mediator,
            DateTime from,
            DateTime to)
        {
            var query = new GetPurchaseInvoiceReportPdfQuery(from, to);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }

        static async Task<Ok<List<PurchaseInvoiceListDTO>>> GetAllByDateRange(
            [FromServices] IMediator mediator,
            DateTime from,
            DateTime to,
            PaginationDTO pagination)
        {
            var query = new GetAllPurchaseInvoicesByDateRangeQuery(from, to, pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<PurchaseInvoiceWithBalanceDTO>> GetAllWithBalance(
            [FromServices] IMediator mediator,
            DateTime from,
            DateTime to,
            PaginationDTO pagination)
        {
            var query = new GetPurchaseInvoicesWithBalanceQuery(from, to, pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}

