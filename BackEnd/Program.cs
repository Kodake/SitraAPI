using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BackEnd.Context;
using Microsoft.OpenApi.Models;
using BackEnd.Helpers;
using FluentValidation;
using BackEnd.Repositories.Products;
using BackEnd.Endpoints;
using BackEnd.Repositories.Finances;
using BackEnd.Repositories.Contacts;
using BackEnd.Repositories.Warehouses;
using BackEnd.Endpoints.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("totalCountRecords");
    });

});

builder.Services.AddOutputCache();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PosAPI",
        Description = "This is an API to work with pos application",
        Contact = new OpenApiContact
        {
            Email = "kodake@hotmail.es",
            Name = "Kodake",
            Url = new Uri("https://github.com/Kodake")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.OperationFilter<AuthorizationFilter>();
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ITaxRepository, TaxRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IPricingItemRepository, PricingItemRepository>();
builder.Services.AddScoped<IPricingRepository, PricingRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api/v1/identity").MapIdentityApi<IdentityUser>();

var apiGroup = app.MapGroup("/api/v1");

apiGroup.MapGroup("/contacts").MapContacts();
apiGroup.MapGroup("/categories").MapCategories();
apiGroup.MapGroup("/brands").MapBrands();
apiGroup.MapGroup("/units").MapUnits();
apiGroup.MapGroup("/currencies").MapCurrencies();
apiGroup.MapGroup("/taxes").MapTaxes();
apiGroup.MapGroup("/warehouses").MapWarehouses();
apiGroup.MapGroup("/products").MapProducts();
apiGroup.MapGroup("/services").MapServices();
apiGroup.MapGroup("/stocks").MapStocks();
apiGroup.MapGroup("/pricings").MapPricings();

app.UseCors();

app.UseOutputCache();

app.Run();