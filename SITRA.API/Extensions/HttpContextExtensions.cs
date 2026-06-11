using Microsoft.AspNetCore.Http;
using System;

namespace SITRA.API.Extensions
{
    public static class HttpContextExtensions
    {
        private const string RefreshTokenCookieName = "refreshToken";

        public static (string? ExpiredJwt, string? RefreshToken) GetRefreshTokens(this HttpContext context)
        {
            var refreshToken = context.Request.Cookies[RefreshTokenCookieName];
            var authHeader = context.Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrEmpty(refreshToken) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return (null, null);
            }
            
            var expiredJwt = authHeader.Substring("Bearer ".Length).Trim();
            return (expiredJwt, refreshToken);
        }

        public static void AppendRefreshTokenCookie(this HttpContext context, string refreshToken, DateTime? expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expiration ?? DateTime.UtcNow.AddDays(7)
            };
            context.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
        }
    }
}
