using BackEnd.MediatR.Handlers.Finances;
using BackEnd.MediatR.Handlers.Products;
using BackEnd.MediatR.Handlers.Contacts;
using BackEnd.MediatR.Handlers.AdministrativeTools;

namespace BackEnd.MediatR
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));

            services.AddBrandServices();
            services.AddUnitServices();
            services.AddCategoryServices();
            services.AddPricingServices();
            services.AddItemsServices();
            services.AddPricingItemServices();
            services.AddCurrencyServices();
            services.AddTaxServices();
            services.AddContactServices();
            services.AddDeadlineServices();
            services.AddFiscalSupportServices();

            return services;
        }
    }
}
