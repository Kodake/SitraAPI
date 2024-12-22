using BackEnd.MediatR.Handlers.Products;

namespace BackEnd.MediatR
{
    public static class MediatRDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRDependencyHandler).Assembly));

            services.AddBrandServices();

            return services;
        }
    }
}
