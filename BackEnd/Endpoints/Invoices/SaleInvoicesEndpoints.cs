using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Helpers;
using BackEnd.Mediator.Commands.Invoices.SaleInvoices;
using BackEnd.Mediator.Queries.Invoices.SaleInvoices;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.Invoices
{
    public static class SaleInvoicesEndpoints
    {
        public static RouteGroupBuilder MapSaleInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/sales-range", GetAllByDateRangeAndBiller).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/sales-report", GenerateReportPdf).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"));
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi();
            return group;
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

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateSaleInvoiceCommand(dto);
            var saleDTO = await mediator.Send(command);
            return saleDTO == null ? TypedResults.NotFound() : TypedResults.Created();
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