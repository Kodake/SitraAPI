using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Helpers
{
    public static class HttpContextExtensionsUtilities
    {
        public static T ExtractDefaultValue<T>(
            this HttpContext context, 
            string fieldName, 
            T defaultValue) where T : IParsable<T>
        {
            var value = context.Request.Query[fieldName];

            if (value.IsNullOrEmpty())
            {
                return defaultValue;
            }

            return T.Parse(value!, null);
        }
    }
}
