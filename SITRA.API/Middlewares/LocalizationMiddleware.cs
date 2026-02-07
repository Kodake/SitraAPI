using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using System.Globalization;

namespace SITRA.API.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LocalizationMiddleware> _logger;

        public LocalizationMiddleware(RequestDelegate next, ILogger<LocalizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        using var scope = context.RequestServices.CreateScope();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                        var user = await userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var culture = new CultureInfo(user.Language.ToString());
                            CultureInfo.CurrentCulture = culture;
                            CultureInfo.CurrentUICulture = culture;

                            _logger.LogDebug($"Set culture to {user.Language} for user {userId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error setting culture for user {userId}");
                    }
                }
            }

            await _next(context);
        }
    }
}
