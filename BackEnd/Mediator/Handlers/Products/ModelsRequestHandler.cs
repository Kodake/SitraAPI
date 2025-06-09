using BackEnd.DTOs.Products.Models;
using BackEnd.Mediator.Commands.Products.Models;
using BackEnd.Mediator.Queries.Products.Models;
using MediatR;

namespace BackEnd.Mediator.Handlers.Products
{
    public static class ModelsRequestHandler
    {
        public static IServiceCollection AddModelServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateModelCommand, bool>, ToggleCommandHandler<ActivateModelCommand>>();
            services.AddTransient<IRequestHandler<DeactivateModelCommand, bool>, ToggleCommandHandler<DeactivateModelCommand>>();
            services.AddTransient<IRequestHandler<GetModelByIdQuery, ModelDTO>, QueryHandler<GetModelByIdQuery, ModelDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsModelsQuery, List<ModelListDTO>>, QueryHandler<GetSelectsModelsQuery, List<ModelListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllModelsQuery, List<ModelListDTO>>, QueryHandler<GetAllModelsQuery, List<ModelListDTO>>>();
            services.AddTransient<IRequestHandler<CreateModelCommand, CreateModelDTO>, CreateCommandHandler<Entities.Products.Model, CreateModelDTO>>();
            services.AddTransient<IRequestHandler<UpdateModelCommand, bool>, UpdateCommandHandler<Entities.Products.Model, CreateModelDTO>>();

            return services;
        }
    }
}
