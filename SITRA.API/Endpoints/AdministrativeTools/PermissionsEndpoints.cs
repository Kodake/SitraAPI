using Mediator;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Roles;

namespace SITRA.API.Endpoints.AdministrativeTools
{
    public static class PermissionsEndpoints
    {
        public static RouteGroupBuilder MapPermissions(this RouteGroupBuilder group)
        {
            group.MapGet("/config", async (IMediator mediator) =>
            {
                var result = await mediator.Send(new GetPermissionConfigQuery());
                return Results.Ok(result);
            });

            return group;
        }
    }
}
