using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace SITRA.Core.Helpers
{
    public static class HttpContextExtensionsUtilities
    {
        public static T ExtractDefaultValue<T>(
            this HttpContext context,
            string fieldName,
            T defaultValue) where T : IParsable<T>
        {
            var value = context.Request.Query[fieldName];

            if (Microsoft.Extensions.Primitives.StringValues.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return T.Parse(value!, null);
        }
    }
}

