using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Invoices.SaleInvoices;
using BackEnd.MediatR.Queries.Invoices.SaleInvoices;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.Invoices
{
    public static class SaleInvoicesEndpoints
    {
        public static RouteGroupBuilder MapInvoices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("saleInvoices-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
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

        public static async Task<IResult> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IMediator mediator)
        {
            var query = new GetSaleInvoicePdfQuery(invoiceId);
            var (fileBytes, fileName) = await mediator.Send(query);
            return Results.File(fileBytes, "application/pdf", fileName);
        }
    }
}