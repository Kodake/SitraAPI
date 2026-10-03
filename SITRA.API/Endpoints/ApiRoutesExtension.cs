using SITRA.API.Endpoints;
using SITRA.API.Endpoints.Admin;
using SITRA.API.Endpoints.AdministrativeTools;
using SITRA.API.Endpoints.Auths;
using SITRA.API.Endpoints.CashBoxes;
using SITRA.API.Endpoints.Invoices;
using SITRA.API.Endpoints.Products;
using SITRA.API.Endpoints.Storage;

public static class ApiRouteExtensions
{
    public static IEndpointRouteBuilder MapApiRoutes(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints.MapGroup("/api/v1").RequireRateLimiting("fixed");

        var identityGroup = apiGroup.MapGroup("/identity");
        identityGroup.MapAuths();
        apiGroup.MapGroup("/users").MapTenantUsers();
        apiGroup.MapGroup("/admin").MapAdminEndpoints();
        apiGroup.MapGroup("/roles").MapRoles();
        apiGroup.MapGroup("/permissions").MapPermissions();
        apiGroup.MapGroup("/dashboards").MapDashboards();
        apiGroup.MapGroup("/receivables").MapReceivables();
        apiGroup.MapGroup("/payables").MapPayables();
        apiGroup.MapGroup("/contacts").MapContacts();
        apiGroup.MapGroup("/categories").MapCategories();
        apiGroup.MapGroup("/subCategories").MapSubCategories();
        apiGroup.MapGroup("/brands").MapBrands();
        apiGroup.MapGroup("/models").MapModels();
        apiGroup.MapGroup("/currencies").MapCurrencies();
        apiGroup.MapGroup("/taxes").MapTaxes();
        apiGroup.MapGroup("/expenses").MapExpenses();
        apiGroup.MapGroup("/warehouses").MapWarehouses();
        apiGroup.MapGroup("/products").MapProducts();
        apiGroup.MapGroup("/services").MapServices();
        apiGroup.MapGroup("/combos").MapCombos();
        apiGroup.MapGroup("/wizards").MapWizards();
        apiGroup.MapGroup("/items").MapItems();
        apiGroup.MapGroup("/stocks").MapStocks();
        apiGroup.MapGroup("/pricings").MapPricings();
        apiGroup.MapGroup("/pricingItems").MapPricingItems();
        apiGroup.MapGroup("/fiscalSupports").MapFiscalSupports();
        apiGroup.MapGroup("/deadlines").MapDeadlines();
        apiGroup.MapGroup("/transfers").MapTransfers();
        apiGroup.MapGroup("/saleInvoices").MapSaleInvoices();
        apiGroup.MapGroup("/purchaseInvoices").MapPurchaseInvoices();
        apiGroup.MapGroup("logInvoices").MapLogInvoices();
        apiGroup.MapGroup("/quotations").MapQuotations();
        apiGroup.MapGroup("/spotPayments").MapSpotPayments();
        apiGroup.MapGroup("/spotPayables").MapSpotPayables();
        apiGroup.MapGroup("/pettyCashes").MapPettyCashes();
        apiGroup.MapGroup("/pettyCashMovements").MapPettyCashMovements();
        apiGroup.MapGroup("/appointments").MapAppointments();
        apiGroup.MapStorageEndpoints();

        return endpoints;
    }
}
