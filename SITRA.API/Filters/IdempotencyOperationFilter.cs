using Microsoft.OpenApi.Models;
using SITRA.API.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SITRA.API.Filters
{
    public class IdempotencyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var idempotentAttribute = context.MethodInfo?.GetCustomAttributes(true)
                .OfType<IdempotentAttribute>()
                .FirstOrDefault();

            if (idempotentAttribute != null)
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                if (!operation.Parameters.Any(p => p.Name == "X-Idempotency-Key"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Idempotency-Key",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "uuid"
                    },
                    Description = "Unique key to ensure idempotency of the operation."
                });
                }
            }
        }
    }
}
