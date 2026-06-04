using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace SITRA.Core.Mediator
{
    public static class MediatorDependencyHandler
    {
        public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
        {
            services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.InputSanitizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.IdempotencyBehavior<,>));

            return services;
        }
    }
}
