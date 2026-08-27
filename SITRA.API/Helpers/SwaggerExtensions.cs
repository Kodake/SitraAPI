using Microsoft.OpenApi;
using System.Text.Json.Nodes;

namespace SITRA.API.Helpers
{
    public static class SwaggerExtensions
    {
        public static TBuilder AddPaginationParamsOpenAPI<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(opt =>
            {
                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "page",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer,
                        Default = JsonValue.Create(1)
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPerPage",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer,
                        Default = JsonValue.Create(10)
                    }
                });
                return opt;
            });
        }
    }
}
