using MediatR;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;

namespace SITRA.Core.Mediator.Queries.Dashboards
{
    public class GetAllDashboardsQuery : IRequest<DashboardDTO>
    {
    }

    public class GetAllDashboardsQueryHandler : IRequestHandler<GetAllDashboardsQuery, DashboardDTO>
    {
        private readonly IDashboardService _dashboardService;

        public GetAllDashboardsQueryHandler(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<DashboardDTO> Handle(GetAllDashboardsQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetDashboardSummaryAsync(cancellationToken);
        }
    }
}
