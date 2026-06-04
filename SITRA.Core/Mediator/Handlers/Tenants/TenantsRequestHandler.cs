using Mediator;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Mediator.Queries.Tenants;

namespace SITRA.Core.Mediator.Handlers.Tenants
{
    public static class TenantsRequestHandler
    {
        public static IServiceCollection AddTenantServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetAllTenantUsersQuery, List<UserDTO>>, QueryHandler<GetAllTenantUsersQuery, List<UserDTO>>>();
            return services;
        }
    }
}
