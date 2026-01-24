using Microsoft.Extensions.DependencyInjection;
using MediatR;
using SITRA.Core.Mediator.Handlers.Accounts;
using SITRA.Core.Mediator.Handlers.AdministrativeTools;
using SITRA.Core.Mediator.Handlers.Appointments;
using SITRA.Core.Mediator.Handlers.CashBoxes;
using SITRA.Core.Mediator.Handlers.Combos;
using SITRA.Core.Mediator.Handlers.Contacts;
using SITRA.Core.Mediator.Handlers.Dashboards;
using SITRA.Core.Mediator.Handlers.Finances;
using SITRA.Core.Mediator.Handlers.Invoices;
using SITRA.Core.Mediator.Handlers.Products;
using SITRA.Core.Mediator.Handlers.Wizard;
using SITRA.Core.Mediator.Handlers.Warehouses;

namespace SITRA.Core.Mediator
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.InputSanitizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.IdempotencyBehavior<,>));

            services.AddDashboardServices();
            services.AddBrandServices();
            services.AddModelServices();
            services.AddUnitServices();
            services.AddCategoryServices();
            services.AddSubCategoryServices();
            services.AddPricingServices();
            services.AddItemsServices();
            services.AddPricingItemServices();
            services.AddCurrencyServices();
            services.AddTaxServices();
            services.AddExpenseServices();
            services.AddContactServices();
            services.AddDeadlineServices();
            services.AddFiscalSupportServices();
            services.AddServiceServices();
            services.AddProductServices();
            services.AddComboServices();
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
            services.AddAppointmentServices();
            services.AddWizardServices();

            return services;
        }
    }
}

