using MediatR;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Handlers.AdministrativeTools.Roles;

namespace SITRA.Core.Mediator.Handlers.AdministrativeTools
{
    public static class RoleRequestHandlers
    {
        public static IServiceCollection AddRoleServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetRoleByIdQuery, RoleDTO?>, GetRoleByIdHandler>();
            services.AddTransient<IRequestHandler<GetAllRolesQuery, List<RoleListDTO>>, GetAllRolesHandler>();
            services.AddTransient<IRequestHandler<GetSelectsRolesQuery, List<RoleListDTO>>, GetSelectsRolesHandler>();

            services.AddTransient<IRequestHandler<CreateRoleCommand, CreateRoleDTO>, CreateCommandHandler<Role, CreateRoleDTO>>();
            services.AddTransient<IRequestHandler<UpdateRoleCommand, bool>, UpdateCommandHandler<Role, CreateRoleDTO>>();
            services.AddTransient<IRequestHandler<DeleteRoleCommand, bool>, DeleteCommandHandler<DeleteRoleCommand>>();
            services.AddTransient<IRequestHandler<AssignPermissionsCommand, bool>, AssignPermissionsCommandHandler>();

            return services;
        }
    }
}
