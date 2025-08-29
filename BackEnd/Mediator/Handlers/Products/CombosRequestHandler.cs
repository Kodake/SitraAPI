using BackEnd.DTOs.Products.Combos;
using BackEnd.Mediator.Commands.Products.Combos;
using BackEnd.Mediator.Queries.Products.Combos;
using MediatR;

namespace BackEnd.Mediator.Handlers.Combos
{
    public static class CombosRequestHandler
    {
        public static IServiceCollection AddComboServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateComboCommand, bool>, ToggleCommandHandler<ActivateComboCommand>>();
            services.AddTransient<IRequestHandler<DeactivateComboCommand, bool>, ToggleCommandHandler<DeactivateComboCommand>>();
            services.AddTransient<IRequestHandler<GetComboByIdQuery, ComboDTO>, QueryHandler<GetComboByIdQuery, ComboDTO>>();
            services.AddTransient<IRequestHandler<GetAllCombosQuery, List<ComboListDTO>>, QueryHandler<GetAllCombosQuery, List<ComboListDTO>>>();
            services.AddTransient<IRequestHandler<CreateComboCommand, CreateComboDTO>, CreateCommandHandler<Entities.Products.Item, CreateComboDTO>>();
            services.AddTransient<IRequestHandler<UpdateComboCommand, bool>, UpdateCommandHandler<Entities.Products.Item, CreateComboDTO>>();

            return services;
        }
    }
}
