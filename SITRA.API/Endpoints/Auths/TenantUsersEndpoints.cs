using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;

using SITRA.Core.Mediator.Queries.Tenants;
using SITRA.Core.Mediator.Commands.Auths;
namespace SITRA.API.Endpoints.Auths
{
    public static class TenantUsersEndpoints
    {
        public static RouteGroupBuilder MapTenantUsers(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetUsers).RequireAuthorization();
            group.MapPost("/", CreateAssociatedUser).RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<IEnumerable<UserDTO>>, BadRequest>> GetUsers(
            [FromServices] IMediator mediator,
            [FromServices] ICurrentTenantService currentTenantService)
        {
            var tenantId = currentTenantService.TenantId;
            if (!tenantId.HasValue) return TypedResults.BadRequest();

            var users = await mediator.Send(new GetAllTenantUsersQuery(tenantId.Value));
            return TypedResults.Ok(users);
        }


        static async Task<Results<Ok, BadRequest>> CreateAssociatedUser(
            [FromBody] CreateAssociatedUserDTO dto,
            [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(new CreateAssociatedUserCommand(dto));
            return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest();
        }

    }
}
