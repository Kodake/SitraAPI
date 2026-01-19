using MediatR;
using System.Reflection;

namespace SITRA.Core.Mediator.Behaviors
{
    public class InputSanitizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            SanitizeInput(request);
            return await next();
        }

        private void SanitizeInput(TRequest request)
        {
            var stringProperties = request.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

            foreach (var property in stringProperties)
            {
                var value = (string?)property.GetValue(request);
                if (value != null)
                {
                    // 1. Trim whitespace
                    var sanitizedValue = value.Trim();

                    // 2. Strip HTML tags (Basic XSS prevention)
                    sanitizedValue = System.Text.RegularExpressions.Regex.Replace(sanitizedValue, "<.*?>", string.Empty);

                    if (value != sanitizedValue)
                    {
                        property.SetValue(request, sanitizedValue);
                    }
                }
            }
        }
    }
}
