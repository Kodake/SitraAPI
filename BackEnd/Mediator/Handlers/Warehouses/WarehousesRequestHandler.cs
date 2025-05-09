using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Mediator.Commands.Warehouses.Warehouses;
using BackEnd.Mediator.Queries.Warehouses.Warehouses;
using MediatR;

namespace BackEnd.Mediator.Handlers.Warehouses
{
    public static class WarehousesRequestHandler
    {
        public static IServiceCollection AddWarehouseServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<CreateWarehouseCommand, CreateWarehouseDTO>, CreateCommandHandler<Entities.Warehouses.Warehouse, CreateWarehouseDTO>>();
            services.AddTransient<IRequestHandler<UpdateWarehouseCommand, bool>, UpdateCommandHandler<Entities.Warehouses.Warehouse, CreateWarehouseDTO>>();
            services.AddTransient<IRequestHandler<ActivateWarehouseCommand, bool>, ToggleCommandHandler<ActivateWarehouseCommand>>();
            services.AddTransient<IRequestHandler<DeactivateWarehouseCommand, bool>, ToggleCommandHandler<DeactivateWarehouseCommand>>();
            services.AddTransient<IRequestHandler<GetWarehouseStockByProductIdQuery, List<WarehouseListDTO>>, QueryHandler<GetWarehouseStockByProductIdQuery, List<WarehouseListDTO>>>();
            services.AddTransient<IRequestHandler<GetWarehousesByIdQuery, WarehouseDTO>, QueryHandler<GetWarehousesByIdQuery, WarehouseDTO>>();
            services.AddTransient<IRequestHandler<GetUnregisteredWarehouseByIdQuery, List<WarehouseListDTO>>, QueryHandler<GetUnregisteredWarehouseByIdQuery, List<WarehouseListDTO>>>();
            services.AddTransient<IRequestHandler<GetSelectsWarehousesQuery, List<WarehouseListDTO>>, QueryHandler<GetSelectsWarehousesQuery, List<WarehouseListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllWarehousesQuery, List<WarehouseListDTO>>, QueryHandler<GetAllWarehousesQuery, List<WarehouseListDTO>>>();
            return services;
        }
    }
}
