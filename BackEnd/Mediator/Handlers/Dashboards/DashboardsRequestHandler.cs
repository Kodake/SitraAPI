using BackEnd.DTOs;
using BackEnd.Mediator.Queries.Dashboards;
using MediatR;

namespace BackEnd.Mediator.Handlers.Dashboards
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
