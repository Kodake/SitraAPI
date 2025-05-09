using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.PurchaseInvoices;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Invoices.PurchaseInvoices;
using BackEnd.Mediator.Queries.Invoices.PurchaseInvoices;
using BackEnd.Mediator.Queries.Invoices.SaleInvoices;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.Invoices
{
    public static class PurchaseInvoicesEndpoints
    {
        public static RouteGroupBuilder MapPurchaseInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/purchases-range", GetAllByDateRange).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/purchases-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("purchaseInvoices-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi();
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

        static async Task<Ok<List<SaleInvoiceListDTO>>> GetAllByDateRange(
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