using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Mediator.Commands.Tenants;
using SITRA.Core.Mediator.Queries.Tenants;

namespace SITRA.API.Endpoints.Auths
{
    public static class TenantUserEndpoints
    {
        public static RouteGroupBuilder MapTenantUsers(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetUsers).RequireAuthorization();
            group.MapPost("/invite", InviteUser).RequireAuthorization();
            group.MapPost("/complete-registration", CompleteRegistration); // No auth required for completion flow

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

        static async Task<Results<Ok<string>, BadRequest>> InviteUser(
            [FromBody] InviteSubUserDTO dto,
            [FromServices] IMediator mediator,
            [FromServices] ICurrentTenantService currentTenantService,
            [FromServices] IUserService userService)
        {
            var tenantId = currentTenantService.TenantId;
            if (!tenantId.HasValue) return TypedResults.BadRequest();

            var currentUser = await userService.GetUserAsync();
            if (currentUser == null) return TypedResults.BadRequest();

            var token = await mediator.Send(new InviteSubUserCommand(dto, tenantId.Value, currentUser.Id));
            return token != null ? TypedResults.Ok(token) : TypedResults.BadRequest();
        }

        static async Task<Results<Ok, BadRequest>> CompleteRegistration(
            [FromBody] CompleteInvitationDTO dto,
            [FromServices] IMediator mediator)
        {
            var result = await mediator.Send(new CompleteSubUserRegistrationCommand(dto));
            return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest();
        }
    }
}
