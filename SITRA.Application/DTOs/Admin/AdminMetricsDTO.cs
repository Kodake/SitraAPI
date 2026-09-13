using SITRA.Core.Enums;

namespace SITRA.Core.DTOs.Admin
{
    public class AdminMetricsDTO
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DisabledUsers { get; set; }
        public int ActiveMemberships { get; set; }
        public int ExpiringSoonMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int RecentRegistrations { get; set; }
        public Dictionary<string, int> PlanDistribution { get; set; } = new();
        public List<GrowthTrendItemDTO> GrowthTrends { get; set; } = new();
    }

    public class GrowthTrendItemDTO
    {
        public string Period { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
