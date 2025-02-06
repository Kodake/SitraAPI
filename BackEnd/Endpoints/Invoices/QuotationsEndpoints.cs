using BackEnd.DTOs;
using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Helpers;
using BackEnd.MediatR.Commands.Invoices.Quotations;
using BackEnd.MediatR.Queries.Invoices.Quotations;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints.Invoices
{
    public static class QuotationsEndpoints
    {
        public static RouteGroupBuilder MapQuotations(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("quotations-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/{id:long}", GetById);
            group.MapPost("/", Create).DisableAntiforgery().WithOpenApi();
            group.MapPost("/{id:long}/process", Process).DisableAntiforgery().WithOpenApi();
            group.MapPut("/{id:long}", Update).DisableAntiforgery().WithOpenApi();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi();
            return group;
        }

        static async Task<Ok<List<QuotationListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllQuotationsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<QuotationDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetQuotationByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateQuotationCommand(dto);
            var quotationDTO = await mediator.Send(command);
            return quotationDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateQuotationCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Process(
            [FromRoute] long id,
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ProcessQuotationCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<FileContentHttpResult, NotFound>> GeneratePdf(
            [FromRoute] int invoiceId,
            [FromServices] IMediator mediator)
        {
            var query = new GetQuotationPdfQuery(invoiceId);
            var result = await mediator.Send(query);

            return result == null
                ? TypedResults.NotFound()
                : TypedResults.File(result.Value.FileBytes, "application/pdf", result.Value.FileName);
        }
    }
}