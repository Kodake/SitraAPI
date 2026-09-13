using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.Core.DTOs.Admin;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Admin;
using SITRA.Core.Mediator.Queries.Admin;
using System.Security.Claims;

namespace SITRA.API.Endpoints.Admin
{
    public static class AdminEndpoints
    {
        public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder group)
        {
            // SuperAdmin Authorization Filter for all /admin routes
            group.AddEndpointFilter(async (invocationContext, next) =>
            {
                var httpContext = invocationContext.HttpContext;
                var mediator = httpContext.RequestServices.GetRequiredService<IMediator>();
                var ruc = httpContext.User.Claims.FirstOrDefault(c => c.Type == "rucNumber")?.Value;
                var email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;

                var isSuperAdmin = await mediator.Send(new CheckSuperAdminAccessQuery(email, ruc));
                if (!isSuperAdmin)
                {
                    return TypedResults.Forbid();
                }

                return await next(invocationContext);
            });

            group.MapGet("/metrics", GetMetrics).RequireAuthorization();
            group.MapGet("/users", GetUsers).RequireAuthorization();
            group.MapPatch("/users/{id}/enable", EnableUser).RequireAuthorization();
            group.MapPatch("/users/{id}/disable", DisableUser).RequireAuthorization();
            group.MapPut("/users/{id}/subscription", UpdateSubscription).RequireAuthorization();
            group.MapPut("/users/{id}/status", UpdateUserStatus).RequireAuthorization();
            group.MapGet("/check-access", CheckAccess).RequireAuthorization();

            return group;
        }

        static async Task<Ok<AdminMetricsDTO>> GetMetrics(
            [FromServices] IMediator mediator,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetAdminMetricsQuery(fromDate, toDate);
            var metrics = await mediator.Send(query);
            return TypedResults.Ok(metrics);
        }

        static async Task<Ok<object>> GetUsers(
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromQuery] string? searchTerm,
            [FromQuery] string? status,
            [FromQuery] PlanType? plan,
            [FromQuery] string? subscriptionStatus,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetAdminUsersQuery(
                searchTerm,
                status,
                plan,
                subscriptionStatus,
                fromDate,
                toDate,
                pageIndex,
                pageSize);

            var (users, totalCount) = await mediator.Send(query);

            httpContext.Response.Headers.Append("totalCountRecords", totalCount.ToString());

            return TypedResults.Ok(new
            {
                items = users,
                totalCount = totalCount,
                pageIndex = pageIndex,
                pageSize = pageSize
            } as object);
        }

        static async Task<Results<Ok, NotFound>> EnableUser(
            [FromRoute] string id,
            [FromServices] IMediator mediator)
        {
            var command = new EnableUserCommand(id);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<Ok, NotFound, BadRequest<string>>> DisableUser(
            [FromRoute] string id,
            [FromBody] DisableUserDTO? dto,
            [FromServices] IMediator mediator)
        {
            try
            {
                var command = new DisableUserCommand(id, dto?.Reason);
                var success = await mediator.Send(command);
                return success ? TypedResults.Ok() : TypedResults.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        static async Task<Results<Ok, NotFound>> UpdateSubscription(
            [FromRoute] string id,
            [FromBody] UpdateSubscriptionDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UpdateUserSubscriptionCommand(
                id,
                dto.Plan,
                dto.SubscriptionStartDate,
                dto.SubscriptionEndDate);

            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<Ok, NotFound, BadRequest<string>>> UpdateUserStatus(
            [FromRoute] string id,
            [FromBody] UpdateUserStatusDTO dto,
            [FromServices] IMediator mediator)
        {
            try
            {
                var command = new UpdateUserStatusCommand(id, dto);
                var success = await mediator.Send(command);
                return success ? TypedResults.Ok() : TypedResults.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        static Ok<object> CheckAccess()
        {
            return TypedResults.Ok(new
            {
                isSuperAdmin = true,
                organization = "SISTRA SRL"
            } as object);
        }
    }
}

