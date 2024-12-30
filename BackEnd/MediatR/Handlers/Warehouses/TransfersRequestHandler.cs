using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.MediatR.Commands.Warehouses.Transfers;
using BackEnd.MediatR.Queries.Warehouses.Transfers;
using MediatR;

namespace BackEnd.MediatR.Handlers.Warehouses
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