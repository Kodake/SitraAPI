using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using QuestPDF.Infrastructure;
using Serilog;
using SITRA.API.Extensions;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator;
using SITRA.Core.Models.Dgii;
using SITRA.Core.Validations.Products;
using SITRA.Infrastructure.ExternalServices.DGII;
using SITRA.Infrastructure.Persistence;
using SITRA.Infrastructure.Repositories;
using SITRA.Infrastructure.Services;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Infrastructure.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Infrastructure.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Infrastructure.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Infrastructure.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Infrastructure.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Infrastructure.Services.Invoices;
using SITRA.Core.Interfaces.Services.Messaging;
using SITRA.Infrastructure.Services.Messaging;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Appointments;
using SITRA.Infrastructure.Services.Appointments;
using SITRA.Core.Interfaces.Services.Accounts;
using SITRA.Infrastructure.Services.Accounts;
using SITRA.Core.Interfaces.Services.Contacts;
using SITRA.Infrastructure.Services.Contacts;
using SITRA.Infrastructure.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Infrastructure.Services.Storage;
using SITRA.Core.Interfaces.Services.Warehouses;
using SITRA.Infrastructure.Services.Warehouses;
using SITRA.Core.Interfaces.Services.Admin;
using SITRA.Infrastructure.Services.Admin;
using Azure.Identity;
var builder = WebApplication.CreateBuilder(args);

// Suppress Server header to prevent fingerprinting
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

// Azure Key Vault — load secrets in non-Development environments
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = builder.Configuration["KeyVault:Url"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential());
    }
}

QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.EnableCaching = true;

// Opcional para debugging
QuestPDF.Settings.EnableDebugging = builder.Environment.IsDevelopment();

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SITRA", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
    c.OperationFilter<SITRA.API.Filters.IdempotencyOperationFilter>();
});

// DbContext
builder.Services.AddScoped<SITRA.Infrastructure.Persistence.Interceptors.AuditInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var auditInterceptor = sp.GetRequiredService<SITRA.Infrastructure.Persistence.Interceptors.AuditInterceptor>();
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null))
        .AddInterceptors(auditInterceptor);
});

// Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // User Lockout Configuration
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 4;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:Issuer"] 
            ?? builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"] 
            ?? builder.Configuration["Jwt:Issuer"],
        IssuerSigningKeys = Keys.GetAllKeys(builder.Configuration)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
            }
            return Task.CompletedTask;
        }
    };
});

// Dependencies
builder.Services.RegisterRequestHandlers();
builder.Services.RegisterRepositories();

// Explicitly load AutoMapper for the API and Core assemblies to avoid AppDomain crashes
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Program).Assembly);
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDTOValidator>();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();
builder.Services.AddScoped<IPettyCashService, PettyCashService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDeadlineService, DeadlineService>();
builder.Services.AddScoped<IFiscalSupportService, FiscalSupportService>();
builder.Services.AddScoped<IPayableService, PayableService>();
builder.Services.AddScoped<IReceivableService, ReceivableService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ISaleInvoiceService, SaleInvoiceService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// DGII Configuration & Services
builder.Services.Configure<DgiiSettings>(builder.Configuration.GetSection("DgiiTest")); // Dgii
builder.Services.Configure<DgiiSettings>("DgiiTest", builder.Configuration.GetSection("DgiiTest"));
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<IEcfXmlGenerator, EcfXmlGeneratorService>();
builder.Services.AddScoped<ITestCasesReader, JsonTestCasesReaderService>();
builder.Services.AddScoped<ITestCasesReader, TestCasesReaderService>();
builder.Services.AddScoped<IDgiiTestService, DgiiTestService>();
builder.Services.AddScoped<IInvoiceToEcfMapper, InvoiceToEcfMapper>();
var storageProvider = builder.Configuration["DgiiTest:StorageProvider"] 
    ?? builder.Configuration["Dgii:StorageProvider"] 
    ?? "GoogleDrive";

if (string.Equals(storageProvider, "GoogleDrive", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<SITRA.Infrastructure.Services.Storage.GoogleDriveStorageService>();
    builder.Services.AddScoped<IDgiiStorageService>(sp => sp.GetRequiredService<SITRA.Infrastructure.Services.Storage.GoogleDriveStorageService>());
    builder.Services.AddScoped<SITRA.Core.Interfaces.Services.Storage.IFileStorageService>(sp => sp.GetRequiredService<SITRA.Infrastructure.Services.Storage.GoogleDriveStorageService>());
}
else
{
    builder.Services.AddScoped<IDgiiStorageService, DgiiStorageService>();
    builder.Services.AddScoped<SITRA.Core.Interfaces.Services.Storage.IFileStorageService, SITRA.Infrastructure.Services.Storage.AzureFileService>();
}

builder.Services.AddHttpClient<IDgiiApiService, DgiiApiService>();

// Global Exception Handler
builder.Services.AddExceptionHandler<SITRA.API.ExceptionHandlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Rate Limiter
builder.Services.AddRateLimiterConfig(builder.Configuration);

// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .SetIsOriginAllowed(origin =>
              {
                  if (string.IsNullOrWhiteSpace(origin)) return false;
                  if (allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase)) return true;
                  if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                  {
                      return uri.Host.EndsWith("vercel.app", StringComparison.OrdinalIgnoreCase) && 
                             uri.Host.StartsWith("jmpuello-app", StringComparison.OrdinalIgnoreCase);
                  }
                  return false;
              })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("totalCountRecords", "totalcountrecords", "Content-Disposition");
    });
});

var app = builder.Build();

// Ensure DGII XSD schemas are present in blob storage (root 'Schemas/' folder, shared across all environments)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var storageService = scope.ServiceProvider.GetRequiredService<IDgiiStorageService>();
        await storageService.EnsureSchemasUploadedAsync();
        Log.Information("DGII schemas verified/uploaded to blob storage successfully.");
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "EnsureSchemasUploadedAsync failed \u2014 schemas may need to be uploaded manually.");
    }
}

// Optimize for Production: Apply pending migrations
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
                Log.Information("Database migrated successfully.");
            }

            // Seed Permissions
            //var permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();
            //await permissionRepository.SeedPermissionsAsync();
            //Log.Information("Permissions seeded successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating the database. Verify connection string in Azure Configuration.");
        }
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler(); // Add this line to use the exception handler

if (app.Environment.IsDevelopment())
{
    // Patch OpenAPI version for Swagger UI compatibility:
    // Microsoft.OpenApi v2 outputs "3.0.4" which the bundled Swagger UI doesn't recognise yet.
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json", StringComparison.OrdinalIgnoreCase)
            || context.Request.Path.StartsWithSegments("/swagger/v1/swagger.yaml", StringComparison.OrdinalIgnoreCase))
        {
            var originalBody = context.Response.Body;
            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            await next();

            buffer.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(buffer).ReadToEndAsync();
            body = body.Replace("\"openapi\": \"3.0.4\"", "\"openapi\": \"3.0.3\"")
                       .Replace("\"openapi\":\"3.0.4\"", "\"openapi\":\"3.0.3\"")
                       .Replace("openapi: 3.0.4", "openapi: 3.0.3");

            context.Response.Body = originalBody;
            context.Response.ContentLength = System.Text.Encoding.UTF8.GetByteCount(body);
            await context.Response.WriteAsync(body);
        }
        else
        {
            await next();
        }
    });

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SITRA API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
    await next();
});

app.UseCors("AllowSpecific");

app.UseOutputCache();

// IP Blocking Middleware - placed before Authentication to catch 401 responses
app.UseMiddleware<SITRA.API.Middlewares.IpBlockingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Localization Middleware - must be after authentication
app.UseMiddleware<SITRA.API.Middlewares.LocalizationMiddleware>();

app.UseRateLimiter();

app.MapApiRoutes();
app.MapControllers();

app.Run();

