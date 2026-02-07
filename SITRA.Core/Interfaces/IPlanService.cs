using SITRA.Core.Enums;

namespace SITRA.Core.Interfaces
{
    public interface IPlanService
    {
        int GetMaxUsers(PlanType plan);
        int? GetMaxMonthlySales(PlanType plan); // Null means unlimited
        List<Feature> GetPlanFeatures(PlanType plan);
        bool HasFeature(PlanType plan, Feature feature);
    }
}
