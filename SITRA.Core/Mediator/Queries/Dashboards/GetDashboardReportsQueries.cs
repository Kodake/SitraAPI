using Mediator;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.Interfaces.Services.Dashboards;

namespace SITRA.Core.Mediator.Queries.Dashboards
{
    public class GetStockAnalysisQuery : IRequest<StockAnalysisDTO>
    {
        public int Year { get; set; }
    }

    public class GetStockAnalysisQueryHandler : IRequestHandler<GetStockAnalysisQuery, StockAnalysisDTO>
    {
        private readonly IDashboardService _dashboardService;
        public GetStockAnalysisQueryHandler(IDashboardService dashboardService) => _dashboardService = dashboardService;
        public async ValueTask<StockAnalysisDTO> Handle(GetStockAnalysisQuery request, CancellationToken cancellationToken)
            => await _dashboardService.GetStockAnalysisAsync(request.Year, cancellationToken);
    }

    public class GetOverallReportQuery : IRequest<OverallReportDTO> { }

    public class GetOverallReportQueryHandler : IRequestHandler<GetOverallReportQuery, OverallReportDTO>
    {
        private readonly IDashboardService _dashboardService;
        public GetOverallReportQueryHandler(IDashboardService dashboardService) => _dashboardService = dashboardService;
        public async ValueTask<OverallReportDTO> Handle(GetOverallReportQuery request, CancellationToken cancellationToken)
            => await _dashboardService.GetOverallReportAsync(cancellationToken);
    }

    public class GetCashFlowReportQuery : IRequest<CashFlowReportDTO>
    {
        public int Year { get; set; }
    }

    public class GetCashFlowReportQueryHandler : IRequestHandler<GetCashFlowReportQuery, CashFlowReportDTO>
    {
        private readonly IDashboardService _dashboardService;
        public GetCashFlowReportQueryHandler(IDashboardService dashboardService) => _dashboardService = dashboardService;
        public async ValueTask<CashFlowReportDTO> Handle(GetCashFlowReportQuery request, CancellationToken cancellationToken)
            => await _dashboardService.GetCashFlowReportAsync(request.Year, cancellationToken);
    }

    public class GetTopSalesQuery : IRequest<List<TopSaleDTO>>
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class GetTopSalesQueryHandler : IRequestHandler<GetTopSalesQuery, List<TopSaleDTO>>
    {
        private readonly IDashboardService _dashboardService;
        public GetTopSalesQueryHandler(IDashboardService dashboardService) => _dashboardService = dashboardService;
        public async ValueTask<List<TopSaleDTO>> Handle(GetTopSalesQuery request, CancellationToken cancellationToken)
            => await _dashboardService.GetTopSalesAsync(request.Month, request.Year, cancellationToken);
    }

    public class GetTopCustomersQuery : IRequest<List<TopCustomerDTO>>
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class GetTopCustomersQueryHandler : IRequestHandler<GetTopCustomersQuery, List<TopCustomerDTO>>
    {
        private readonly IDashboardService _dashboardService;
        public GetTopCustomersQueryHandler(IDashboardService dashboardService) => _dashboardService = dashboardService;
        public async ValueTask<List<TopCustomerDTO>> Handle(GetTopCustomersQuery request, CancellationToken cancellationToken)
            => await _dashboardService.GetTopCustomersAsync(request.Month, request.Year, cancellationToken);
    }
}
