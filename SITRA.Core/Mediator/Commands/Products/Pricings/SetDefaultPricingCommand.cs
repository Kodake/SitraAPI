using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Pricings
{
    public record SetDefaultPricingCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultPricingCommandHandler : IRequestHandler<SetDefaultPricingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultPricingCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(
            SetDefaultPricingCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.PricingRepository.SetDefault(request.Id);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);

            return true;
        }
    }
}
