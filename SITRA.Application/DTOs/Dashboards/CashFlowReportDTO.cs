namespace SITRA.Core.DTOs.Dashboards
{
    public class CashFlowReportDTO
    {
        public List<string> Months { get; set; } = new();
        public List<double> Income { get; set; } = new();
        public List<double> Expenses { get; set; } = new();
    }
}
