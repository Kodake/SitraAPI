using BackEnd.DTOs;
using BackEnd.MediatR.Queries.Dashboards;
using MediatR;

namespace BackEnd.MediatR.Handlers.Dashboards
{
    public static class DashboardsRequestHandler
    {
        public static IServiceCollection AddDashboardServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetAllDashboardsQuery, List<DashboardListDTO>>, QueryHandler<GetAllDashboardsQuery, List<DashboardListDTO>>>();

            return services;
        }
    }
}
