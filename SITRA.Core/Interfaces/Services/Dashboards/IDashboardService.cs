using SITRA.Core.DTOs.Dashboards;

namespace SITRA.Core.Interfaces.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    }
}
