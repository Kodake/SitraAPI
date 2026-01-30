using MediatR;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Mediator.Commands.Products.Units;
using SITRA.Core.Mediator.Queries.Products.Units;

namespace SITRA.Core.Mediator.Handlers.Products
{
    public static class UnitsRequestHandler
    {
        public static IServiceCollection AddUnitServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateUnitCommand, bool>, ToggleCommandHandler<ActivateUnitCommand>>();
            services.AddTransient<IRequestHandler<DeactivateUnitCommand, bool>, ToggleCommandHandler<DeactivateUnitCommand>>();
            services.AddTransient<IRequestHandler<GetUnitByIdQuery, UnitDTO>, QueryHandler<GetUnitByIdQuery, UnitDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsUnitsQuery, List<UnitListDTO>>, QueryHandler<GetSelectsUnitsQuery, List<UnitListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllUnitsQuery, List<UnitListDTO>>, QueryHandler<GetAllUnitsQuery, List<UnitListDTO>>>();
            services.AddTransient<IRequestHandler<CreateUnitCommand, CreateUnitDTO>, CreateCommandHandler<Entities.Products.Unit, CreateUnitDTO>>();
            services.AddTransient<IRequestHandler<UpdateUnitCommand, bool>, UpdateCommandHandler<Entities.Products.Unit, CreateUnitDTO>>();

            return services;
        }
    }
}

