using BackEnd.Repositories.Generics;
using BackEnd.Repositories;
using System.Reflection;

namespace BackEnd.MediatR
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

            return services;
        }
    }
}
