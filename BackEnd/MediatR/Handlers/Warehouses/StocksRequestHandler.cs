using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.MediatR.Commands.Warehouses.Stocks;
using BackEnd.MediatR.Queries.Warehouses.Stocks;
using MediatR;

namespace BackEnd.MediatR.Handlers.Warehouses
{
    public static class StocksRequestHandler
    {
        public static IServiceCollection AddStockServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetStockByIdQuery, StockDTO>, QueryHandler<GetStockByIdQuery, StockDTO>>();
            services.AddTransient<IRequestHandler<GetAllStocksQuery, List<StockListDTO>>, QueryHandler<GetAllStocksQuery, List<StockListDTO>>>();
            services.AddTransient<IRequestHandler<CreateStockCommand, CreateStockDTO>, CreateCommandHandler<Entities.Warehouses.Stock, CreateStockDTO>>();
            services.AddTransient<IRequestHandler<UpdateStockCommand, bool>, UpdateCommandHandler<Entities.Warehouses.Stock, CreateStockDTO>>();
            return services;
        }
    }
}