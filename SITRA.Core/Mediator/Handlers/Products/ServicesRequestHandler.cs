using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Mediator.Commands.Products.Services;
using SITRA.Core.Mediator.Queries.Products.Services;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Products
{
    public static class ServicesRequestHandler
    {
        public static IServiceCollection AddServiceServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateServiceCommand, bool>, ToggleCommandHandler<ActivateServiceCommand>>();
            services.AddTransient<IRequestHandler<DeactivateServiceCommand, bool>, ToggleCommandHandler<DeactivateServiceCommand>>();
            services.AddTransient<IRequestHandler<GetServiceByIdQuery, ServiceDTO>, QueryHandler<GetServiceByIdQuery, ServiceDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsServicesQuery, List<ServiceListDTO>>, QueryHandler<GetSelectsServicesQuery, List<ServiceListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllServicesQuery, List<ServiceListDTO>>, QueryHandler<GetAllServicesQuery, List<ServiceListDTO>>>();
            services.AddTransient<IRequestHandler<CreateServiceCommand, CreateServiceDTO>, CreateCommandHandler<Entities.Products.Item, CreateServiceDTO>>();
            services.AddTransient<IRequestHandler<UpdateServiceCommand, bool>, UpdateCommandHandler<Entities.Products.Item, CreateServiceDTO>>();
            services.AddTransient<IRequestHandler<UploadCsvServiceCommand, bool>, UploadCsvServiceCommand.Handler>();

            return services;
        }
    }
}

