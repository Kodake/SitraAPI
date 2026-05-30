using SITRA.Core.Interfaces.Services.Messaging;
using SITRA.Infrastructure.Services.Messaging;
using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Generics;
using SITRA.Infrastructure.Repositories.Generics;
using System.Reflection;

namespace SITRA.Infrastructure.Repositories
{
    public static class RepositoryDependencyHandler
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var repositoryTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Repository") && !t.IsInterface)
                .ToList();

            foreach (var repo in repositoryTypes)
            {
                var interfaceType = repo.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + repo.Name);

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, repo);
                }
            }

            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<SITRA.Core.Interfaces.Services.Messaging.IEmailService, SITRA.Infrastructure.Services.Messaging.EmailService>();

            return services;
        }
    }
}
