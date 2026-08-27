using Microsoft.OpenApi;
using SITRA.API.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SITRA.API.Filters;

public class IdempotencyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var idempotentAttribute = context.MethodInfo?.GetCustomAttributes(true)
            .OfType<IdempotentAttribute>()
            .FirstOrDefault();

        if (idempotentAttribute != null)
        {
            operation.Parameters ??= new List<IOpenApiParameter>();

            if (!operation.Parameters.Any(p => p.Name == "X-Idempotency-Key"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Idempotency-Key",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Format = "uuid"
                    },
                    Description = SITRA.Core.Helpers.Utilities.IdempotencyKeyDescriptionMessage
                });
            }
        }
    }
}


