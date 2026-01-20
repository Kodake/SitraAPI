using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.API.Helpers;
using SITRA.API.Filters;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Queries.Invoices.SaleInvoices;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Attributes;

namespace SITRA.API.Endpoints.Invoices
{
    public static class SaleInvoicesEndpoints
    {
        public static RouteGroupBuilder MapSaleInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/sales-range", GetAllByDateRangeAndBiller).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/sales-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get")).RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapGet("/summary", GetBalance).WithOpenApi().RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateSaleInvoiceDTO>>().WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-receipt-pdf", GenerateReceiptPdf).WithOpenApi().RequireAuthorization();
            return group;
        }

        static async Task<Ok<BalanceSummaryDTO>> GetBalance(
            [FromServices] IMediator mediator,
            DateTime from,
            DateTime to)
        {
            var query = new GetSaleInvoicesBalanceQuery(from, to);
            var result = await mediator.Send(query);
            return TypedResults.Ok(result);
        }

        static async Task<Ok<List<SaleInvoiceListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllSaleInvoicesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<SaleInvoiceDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetSaleInvoiceByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        [Idempotent]
        static async Task<Results<Created<CreateSaleInvoiceDTO>, NotFound, ValidationProblem>> Create(
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateSaleInvoiceCommand(dto);
            var saleDTO = await mediator.Send(command);

            return saleDTO == null
                ? TypedResults.NotFound()
                : TypedResults.Created(string.Empty, saleDTO);
        }

        static async Task<Results<FileContentHttpResult, NotFound>> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IMediator mediator)
        {
            var query = new GetSaleInvoicePdfQuery(invoiceId);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }

        static async Task<Results<FileContentHttpResult, NotFound>> GenerateReceiptPdf(
            [FromRoute] int invoiceId,
            [FromServices] IMediator mediator)
        {
            var query = new GetSaleInvoiceReceiptPdfQuery(invoiceId);
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
            var query = new GetSaleInvoiceReportPdfQuery(from, to);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }

        static async Task<Ok<List<SaleInvoiceListDTO>>> GetAllByDateRangeAndBiller(
            [FromServices] IMediator mediator,
            DateTime from,
            DateTime to,
            PaginationDTO pagination)
        {
            var query = new GetAllSaleInvoicesByDateRangeAndBillerQuery(from, to, pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }
    }
}

