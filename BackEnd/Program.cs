using BackEnd.Context;
using BackEnd.Entities.Auths;
using BackEnd.Helpers;
using BackEnd.Mediator;
using BackEnd.MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//
// ✅ Configure built-in logging
//
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddApplicationInsights();

//
// ✅ Database
//
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging();
});

//
// ✅ Services
//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

// 🔐 AQUÍ VA EL BEARER
builder.Services
.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKeys = Keys.GetAllKeys(builder.Configuration),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion
            .WithOrigins(
                "https://rpos-app-git-pre-prod-kodakes-projects.vercel.app",
                "https://2a0405c99c38.ngrok-free.app",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("totalCountRecords");
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddOutputCache();

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

builder.Services.RegisterRequestHandlers();
builder.Services.RegisterRepositories();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();

builder.Services.AddApplicationInsightsTelemetry(
    new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
    {
        ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    }
);

var app = builder.Build();

//
// ✅ Global exception handler
//
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error != null)
        {
            logger.LogError(exceptionHandlerPathFeature.Error,
                "Unhandled exception on path {Path}",
                exceptionHandlerPathFeature.Path);
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

//
// ✅ Database migration + logging
//
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (dbContext.Database.IsRelational())
    {
        try
        {
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Database migrated successfully.");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2714)
        {
            logger.LogWarning("⚠️ Identity tables already exist. Skipping duplicate creation.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Migration failed.");
            throw;
        }
    }
}

//
// Middleware pipeline
//
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapApiRoutes();
app.UseOutputCache();
app.UseAuthorization();

app.Run();
