using MediatR;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.Mediator.Queries.Dashboards;

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

