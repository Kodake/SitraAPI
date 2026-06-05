using SITRA.Core.DTOs.Dashboards;

namespace SITRA.Core.Interfaces.Services.Dashboards
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
        Task<StockAnalysisDTO> GetStockAnalysisAsync(int year, CancellationToken cancellationToken = default);
        Task<OverallReportDTO> GetOverallReportAsync(CancellationToken cancellationToken = default);
        Task<CashFlowReportDTO> GetCashFlowReportAsync(int year, CancellationToken cancellationToken = default);
        Task<List<TopSaleDTO>> GetTopSalesAsync(int month, int year, CancellationToken cancellationToken = default);
    }
}
