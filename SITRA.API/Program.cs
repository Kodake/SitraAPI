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
using SITRA.Core.Validations;
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
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Authentication:Schemes:Bearer:Issuer"],
        IssuerSigningKeys = Keys.GetAllKeys(builder.Configuration)
    };
});

// Dependencies
builder.Services.RegisterRequestHandlers();
builder.Services.RegisterRepositories();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDTOValidator>();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseOutputCache();

app.UseMiddleware<SITRA.API.Middlewares.IpBlockingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Localization Middleware
app.Use(async (context, next) =>
{
    var languageClaim = context.User.FindFirst("language")?.Value;
    if (!string.IsNullOrEmpty(languageClaim))
    {
        var culture = new System.Globalization.CultureInfo(languageClaim);
        System.Globalization.CultureInfo.CurrentCulture = culture;
        System.Globalization.CultureInfo.CurrentUICulture = culture;
    }
    await next();
});

app.MapApiRoutes();

app.Run();
