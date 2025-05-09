using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Mediator.Commands.Warehouses.Transfers;
using BackEnd.Mediator.Queries.Warehouses.Transfers;
using MediatR;

namespace BackEnd.Mediator.Handlers.Warehouses
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