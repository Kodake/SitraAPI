namespace SITRA.Core.DTOs.Dashboards
{
    public class StockAnalysisDTO
    {
        public List<string> Months { get; set; } = new();
        public List<double> PurchasedStock { get; set; } = new();
        public List<double> SoldStock { get; set; } = new();
    }
}
