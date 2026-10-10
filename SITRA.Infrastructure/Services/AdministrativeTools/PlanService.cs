using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services.Messaging;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Infrastructure.Services.AdministrativeTools
{
    public class PlanService : IPlanService
    {
        public int GetMaxUsers(PlanType plan)
        {
            return plan switch
            {
                PlanType.Free => 1,
                PlanType.Standard => 2,
                PlanType.Premium => 5,
                PlanType.Full => int.MaxValue, // Unlimited
                _ => 1
            };
        }

        public int? GetMaxMonthlySales(PlanType plan)
        {
            return plan switch
            {
                PlanType.Free => 10,
                PlanType.Standard => 100,
                PlanType.Premium => 300,
                PlanType.Full => null, // Unlimited
                _ => 10
            };
        }

        public List<Feature> GetPlanFeatures(PlanType plan)
        {
            var features = new List<Feature> { Feature.SalesOverview };

            if (plan >= PlanType.Standard)
            {
                features.Add(Feature.InventoryManagement);
                features.Add(Feature.AdvancedReports);
            }

            if (plan >= PlanType.Premium)
            {
                features.Add(Feature.ApiIntegrations);
            }

            if (plan >= PlanType.Full)
            {
                features.Add(Feature.CustomReports);
                features.Add(Feature.AdvancedAnalytics);
            }

            return features.Distinct().ToList();
        }

        public bool HasFeature(PlanType plan, Feature feature)
        {
            return GetPlanFeatures(plan).Contains(feature);
        }
    }
}
