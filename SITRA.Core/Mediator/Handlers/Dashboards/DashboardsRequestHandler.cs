using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs;
using SITRA.Core.Mediator.Queries.Dashboards;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Dashboards
{
    public static class DashboardsRequestHandler
    {
        public static IServiceCollection AddDashboardServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetAllDashboardsQuery, DashboardDTO>, QueryHandler<GetAllDashboardsQuery, DashboardDTO>>();

            return services;
        }
    }
}

