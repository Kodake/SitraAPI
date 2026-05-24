using MediatR;
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

            return group;
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