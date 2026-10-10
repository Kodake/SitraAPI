using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.API.Attributes;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Invoices.Quotations;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Commands.Invoices.Quotations;
using SITRA.Core.Mediator.Queries.Invoices.Quotations;

namespace SITRA.API.Endpoints.Invoices
{
    public static class QuotationsEndpoints
    {
        public static RouteGroupBuilder MapQuotations(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("quotations-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateQuotationDTO>>().WithOpenApi().RequireAuthorization();
            group.MapPost("/with-item", CreateWithProductOrService).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateQuotationWithItemDTO>>().WithOpenApi().RequireAuthorization();
            group.MapPost("/{id:guid}/process", Process).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateSaleInvoiceDTO>>().WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateQuotationDTO>>().WithOpenApi().RequireAuthorization();
            group.MapGet("/{invoiceId}/generate-pdf", GeneratePdf).WithOpenApi().RequireAuthorization();
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
            Guid id)
        {
            var query = new GetQuotationByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        [Idempotent]
        static async Task<Results<Created, NotFound, ValidationProblem>> CreateWithProductOrService(
            [FromBody] CreateQuotationWithItemDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateQuotationWithItemCommand(dto);
            var quotationDTO = await mediator.Send(command);
            return quotationDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        [Idempotent]
        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateQuotationCommand(dto);
            var quotationDTO = await mediator.Send(command);
            return quotationDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        [Idempotent]
        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            Guid id,
            [FromBody] CreateQuotationDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateQuotationCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        [Idempotent]
        static async Task<Results<Created<CreateSaleInvoiceDTO>, NotFound, ValidationProblem>> Process(
            [FromRoute] Guid id,
            [FromBody] CreateSaleInvoiceDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ProcessQuotationCommand(id, dto);
            var saleDTO = await mediator.Send(command);

            return saleDTO == null
                ? TypedResults.NotFound()
                : TypedResults.Created(string.Empty, saleDTO);
        }

        static async Task<IResult> GeneratePdf(
            [FromRoute] Guid invoiceId,
            [FromServices] IMediator mediator)
        {
            try
            {
                var query = new GetQuotationPdfQuery(invoiceId);
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
    }
}

