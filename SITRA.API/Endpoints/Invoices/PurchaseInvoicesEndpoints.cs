using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

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
            group.MapGet("/purchases-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get")).RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi().RequireAuthorization();
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
            long id)
        {
            var query = new GetPurchaseInvoiceByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreatePurchaseInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreatePurchaseInvoiceCommand(dto);
            var saleDTO = await mediator.Send(command);
            return saleDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<FileContentHttpResult, NotFound>> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IMediator mediator)
        {
            var query = new GetPurchaseInvoicePdfQuery(invoiceId);
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
    }
}

