using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.Auths;

using SITRA.Core.Mediator.Queries.Tenants;

namespace SITRA.Core.Mediator.Handlers.Tenants
{
    public static class TenantsRequestHandler
    {
        public static IServiceCollection AddTenantServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetAllTenantUsersQuery, IEnumerable<UserDTO>>, GetAllTenantUsersHandler>();
            return services;
        }
    }
}
