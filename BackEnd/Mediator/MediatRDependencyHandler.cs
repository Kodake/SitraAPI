using BackEnd.Mediator.Handlers.Accounts;
using BackEnd.Mediator.Handlers.AdministrativeTools;
using BackEnd.Mediator.Handlers.CashBoxes;
using BackEnd.Mediator.Handlers.Contacts;
using BackEnd.Mediator.Handlers.Dashboards;
using BackEnd.Mediator.Handlers.Finances;
using BackEnd.Mediator.Handlers.Invoices;
using BackEnd.Mediator.Handlers.Products;
using BackEnd.Mediator.Handlers.Warehouses;

namespace BackEnd.Mediator
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));

            services.AddDashboardServices();
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
            services.AddServiceServices();
            services.AddStockServices();
            services.AddTransferServices();
            services.AddWarehouseServices();
            services.AddReceivableServices();
            services.AddPayableServices();
            services.AddSpotPaymentsServices();
            services.AddSpotPayablesServices();
            services.AddQuotationsServices();
            services.AddSaleInvoicesServices();
            services.AddPurchaseInvoicesServices();
            services.AddCashBoxServices();

            return services;
        }
    }
}
