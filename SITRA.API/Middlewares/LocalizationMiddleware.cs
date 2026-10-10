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
            string? language = null;

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
                            language = user.Language.ToString();
                            _logger.LogDebug($"Set culture to {user.Language} from user profile {userId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error getting culture for user {userId}");
                    }
                }
            }

            if (string.IsNullOrEmpty(language))
            {
                var acceptLanguage = context.Request.Headers["Accept-Language"].ToString();
                if (!string.IsNullOrEmpty(acceptLanguage))
                {
                    var preferredLanguage = acceptLanguage.Split(',').FirstOrDefault()?.Split(';').FirstOrDefault()?.Trim();
                    if (!string.IsNullOrEmpty(preferredLanguage))
                    {
                        if (preferredLanguage.Equals("es", StringComparison.OrdinalIgnoreCase) || preferredLanguage.StartsWith("es-", StringComparison.OrdinalIgnoreCase))
                        {
                            language = "es";
                        }
                        else if (preferredLanguage.Equals("en", StringComparison.OrdinalIgnoreCase) || preferredLanguage.StartsWith("en-", StringComparison.OrdinalIgnoreCase))
                        {
                            language = "en";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(language))
            {
                try
                {
                    var culture = new CultureInfo(language);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error setting culture to {language}");
                }
            }

            await _next(context);
        }
    }
}
