using Mediator;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.Mediator.Commands.Warehouses.Transfers;
using SITRA.Core.Mediator.Queries.Warehouses.Transfers;

namespace SITRA.Core.Mediator.Handlers.Warehouses
{
    public static class TransfersRequestHandler
    {
        public static IServiceCollection AddTransferServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<GetTransferByIdQuery, TransferDTO>, QueryHandler<GetTransferByIdQuery, TransferDTO>>();
            services.AddTransient<IRequestHandler<GetAllTransfersQuery, List<TransferListDTO>>, QueryHandler<GetAllTransfersQuery, List<TransferListDTO>>>();
            services.AddTransient<IRequestHandler<CreateTransferCommand, CreateTransferDTO>, CreateCommandHandler<Entities.Warehouses.Transfer, CreateTransferDTO>>();
            services.AddTransient<IRequestHandler<UpdateTransferStatusCommand, bool>, UpdateCommandHandler<Entities.Warehouses.Transfer, CreateTransferStatusDTO>>();
            return services;
        }
    }
}

