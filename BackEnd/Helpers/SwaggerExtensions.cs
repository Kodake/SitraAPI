using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace BackEnd.Helpers
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
                        Type = "integer",
                        Default = new OpenApiInteger(1)
                    }
                });

                opt.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPerPage",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });
                return opt;
            });
        }
    }
}
