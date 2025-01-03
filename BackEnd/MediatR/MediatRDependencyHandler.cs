using BackEnd.MediatR.Handlers.Accounts;
using BackEnd.MediatR.Handlers.AdministrativeTools;
using BackEnd.MediatR.Handlers.Contacts;
using BackEnd.MediatR.Handlers.Finances;
using BackEnd.MediatR.Handlers.Products;
using BackEnd.MediatR.Handlers.Warehouses;

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
            services.AddProductServices();
            services.AddStockServices();
            services.AddTransferServices();
            services.AddWarehouseServices();
            services.AddReceivableServices();

            return services;
        }
    }
}
