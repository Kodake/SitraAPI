using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Roles;

namespace SITRA.API.Endpoints.AdministrativeTools
{
    public static class RolesEndpoints
    {
        public static RouteGroupBuilder MapRoles(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("roles-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateRoleDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateRoleDTO>>()
                .WithOpenApi().RequireAuthorization();
            group.MapDelete("/{id:guid}", Delete).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<RoleListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllRolesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<RoleDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetRoleByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<Results<Created, NotFound, ValidationProblem>> Create(
            [FromBody] CreateRoleDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new CreateRoleCommand(dto);
            var roleDTO = await mediator.Send(command);
            return roleDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(
            Guid id,
            [FromBody] CreateRoleDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateRoleCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, NoContent>> Delete(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeleteRoleCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
