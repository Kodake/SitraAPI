using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
var builder = WebApplication.CreateBuilder(args);

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
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.OperationFilter<SITRA.API.Filters.IdempotencyOperationFilter>();
});

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<User, IdentityRole>()
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
    options.RequireHttpsMetadata = false;
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
            Console.WriteLine($"[DEBUG] Auth Header='{authHeader}'");
            // If we manually extract token here, let's see what happens
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
            }
            Console.WriteLine($"[DEBUG] context.Token='{context.Token}'");
            return Task.CompletedTask;
        }
    };
});

// Dependencies
builder.Services.RegisterRequestHandlers();
builder.Services.RegisterRepositories();

// Explicitly load AutoMapper for the API and Core assemblies to avoid AppDomain crashes
builder.Services.AddAutoMapper(config => { }, typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDTOValidator>();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<IPlanService, PlanService>();

// DGII Configuration & Services
builder.Services.Configure<DgiiSettings>(builder.Configuration.GetSection("Dgii"));

builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddScoped<IEcfXmlGenerator, EcfXmlGeneratorService>();
builder.Services.AddScoped<ITestCasesReader, JsonTestCasesReaderService>();
builder.Services.AddScoped<ITestCasesReader, TestCasesReaderService>();
builder.Services.AddScoped<IDgiiTestService, DgiiTestService>();
builder.Services.AddScoped<IInvoiceToEcfMapper, InvoiceToEcfMapper>();

builder.Services.AddHttpClient<IDgiiApiService, DgiiApiService>();

// Global Exception Handler
builder.Services.AddExceptionHandler<SITRA.API.ExceptionHandlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Rate Limiter
builder.Services.AddRateLimiterConfig(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("totalcountrecords");
    });
});

var app = builder.Build();

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
            Log.Error(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler(); // Add this line to use the exception handler

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

// Localization Middleware - must be after authentication
app.UseMiddleware<SITRA.API.Middlewares.LocalizationMiddleware>();

app.UseRateLimiter();

app.MapApiRoutes();
app.MapControllers();

app.Run();
