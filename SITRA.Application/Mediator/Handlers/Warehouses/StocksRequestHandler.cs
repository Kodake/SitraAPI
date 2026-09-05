using Mediator;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Mediator.Commands.Warehouses.Stocks;
using SITRA.Core.Mediator.Queries.Warehouses.Stocks;

namespace SITRA.Core.Mediator.Handlers.Warehouses
{
    public static class StocksRequestHandler
    {
        public static IServiceCollection AddStockServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetStockByIdQuery, StockDTO>, QueryHandler<GetStockByIdQuery, StockDTO>>();

            services.AddTransient<IRequestHandler<GetAllStocksByWarehouseIdsQuery, List<StockListDTO>>, QueryHandler<GetAllStocksByWarehouseIdsQuery, List<StockListDTO>>>();
            services.AddTransient<IRequestHandler<GetStockReportPdfQuery, (byte[] FileBytes, string FileName)?>, QueryHandler<GetStockReportPdfQuery, (byte[] FileBytes, string FileName)?>>();
            services.AddTransient<IRequestHandler<CreateStockCommand, CreateStockDTO>, CreateCommandHandler<Entities.Warehouses.Stock, CreateStockDTO>>();
            services.AddTransient<IRequestHandler<UpdateStockCommand, bool>, UpdateCommandHandler<Entities.Warehouses.Stock, CreateStockDTO>>();
            return services;
        }
    }
}

