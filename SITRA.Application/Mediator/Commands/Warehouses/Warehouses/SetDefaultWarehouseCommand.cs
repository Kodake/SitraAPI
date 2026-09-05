using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Warehouses.Warehouses
{
    public record SetDefaultWarehouseCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultWarehouseCommandHandler : IRequestHandler<SetDefaultWarehouseCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultWarehouseCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async ValueTask<bool> Handle(
            SetDefaultWarehouseCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.WarehouseRepository.SetDefault(request.Id);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("warehouses-get", cancellationToken);

            return true;
        }
    }
}
