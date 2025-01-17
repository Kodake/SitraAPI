using BackEnd.Endpoints;
using BackEnd.Endpoints.Invoices;
using BackEnd.Endpoints.Products;
using Microsoft.AspNetCore.Identity;

public static class ApiRouteExtensions
{
    public static IEndpointRouteBuilder MapApiRoutes(this IEndpointRouteBuilder endpoints)
    {
        var apiGroup = endpoints.MapGroup("/api/v1");

        apiGroup.MapGroup("/identity").MapIdentityApi<IdentityUser>();
        apiGroup.MapGroup("/receivables").MapReceivables();
        apiGroup.MapGroup("/contacts").MapContacts();
        apiGroup.MapGroup("/categories").MapCategories();
        apiGroup.MapGroup("/brands").MapBrands();
        apiGroup.MapGroup("/units").MapUnits();
        apiGroup.MapGroup("/currencies").MapCurrencies();
        apiGroup.MapGroup("/taxes").MapTaxes();
        apiGroup.MapGroup("/warehouses").MapWarehouses();
        apiGroup.MapGroup("/products").MapProducts();
        apiGroup.MapGroup("/services").MapServices();
        apiGroup.MapGroup("/items").MapItems();
        apiGroup.MapGroup("/stocks").MapStocks();
        apiGroup.MapGroup("/pricings").MapPricings();
        apiGroup.MapGroup("/pricingItems").MapPricingItems();
        apiGroup.MapGroup("/fiscalSupports").MapFiscalSupports();
        apiGroup.MapGroup("/deadlines").MapDeadlines();
        apiGroup.MapGroup("/transfers").MapTransfers();
        apiGroup.MapGroup("/saleInvoices").MapInvoices();
        apiGroup.MapGroup("/spotPayments").MapSpotPayments();

        return endpoints;
    }
}
