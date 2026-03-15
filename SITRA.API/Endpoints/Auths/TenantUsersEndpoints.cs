using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Queries.Tenants;
namespace SITRA.API.Endpoints.Auths
{
    public static class TenantUsersEndpoints
    {
        public static RouteGroupBuilder MapTenantUsers(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetUsers).RequireAuthorization();
            group.MapGet("/{id}", GetUserById).RequireAuthorization();
            group.MapPost("/", CreateAssociatedUser).RequireAuthorization()
                .AddEndpointFilter<ValidationFilter<CreateAssociatedUserDTO>>()
                .WithOpenApi();
            group.MapPut("/{id}/role", UpdateAssociatedUserRole).RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<List<UserDTO>>, BadRequest>> GetUsers(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var users = await mediator.Send(new GetAllTenantUsersQuery(pagination));
            return TypedResults.Ok(users);
        }

        static async Task<Results<Ok<UserDTO>, NotFound, BadRequest>> GetUserById(
            [FromRoute] string id,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentTenantService currentTenantService)
        {
            var tenantId = currentTenantService.TenantId;
            if (!tenantId.HasValue) return TypedResults.BadRequest();

            var user = await mediator.Send(new GetUserByIdQuery(tenantId.Value, id));
            return user != null ? TypedResults.Ok(user) : TypedResults.NotFound();
        }


        static async Task<Results<Ok, BadRequest>> CreateAssociatedUser(
            [FromBody] CreateAssociatedUserDTO dto,
            [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(new CreateAssociatedUserCommand(dto));
            return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest();
        }

        static async Task<Results<Ok, BadRequest>> UpdateAssociatedUserRole(
            [FromRoute] string id,
            [FromBody] UpdateUserRoleDTO dto,
            [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(new UpdateAssociatedUserRoleCommand(id, dto));
            return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest();
        }

    }
}
