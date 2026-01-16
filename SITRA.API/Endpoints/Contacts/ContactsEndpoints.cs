using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.Mediator.Commands.Contacts;
using SITRA.Core.Mediator.Queries.Contacts;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.API.Endpoints
{
    public static class ContactsEndpoints
    {
        public static RouteGroupBuilder MapContacts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("contacts-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/suppliers", GetSuppliers).RequireAuthorization();
            group.MapGet("/customers", GetCustomers).RequireAuthorization();
            group.MapGet("/billers", GetBillers).RequireAuthorization();
            group.MapGet("/{id:long}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateContactDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:long}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateContactDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:long}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:long}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ContactListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllContactsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ContactListDTO>>> GetSuppliers(
            [FromServices] IMediator mediator)
        {
            var query = new GetSelectsSuppliersQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ContactListDTO>>> GetCustomers(
            [FromServices] IMediator mediator)
        {
            var query = new GetSelectsCustomersQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ContactListDTO>>> GetBillers(
            [FromServices] IMediator mediator)
        {
            var query = new GetSelectsBillersQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ContactDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            long id)
        {
            var query = new GetContactByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateContactDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new CreateContactCommand(dto);
            var contactDTO = await mediator.Send(command);
            return contactDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            long id,
            [FromBody] CreateContactDTO dto,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new UpdateContactCommand(id, dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new ActivateContactCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] long id,
            [FromServices] IMediator mediator,
            IOutputCacheStore outputCacheStore)
        {
            var command = new DeactivateContactCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

