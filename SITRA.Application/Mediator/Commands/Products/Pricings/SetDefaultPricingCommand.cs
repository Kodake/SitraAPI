using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Commands.Products.Pricings
{
    public record SetDefaultPricingCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultPricingCommandHandler : IRequestHandler<SetDefaultPricingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPricingService _pricingService;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultPricingCommandHandler(
            IUnitOfWork unitOfWork,
            IPricingService pricingService,
            IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _pricingService = pricingService;
            _outputCacheStore = outputCacheStore;
        }

        public async ValueTask<bool> Handle(
            SetDefaultPricingCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _pricingService.SetDefaultAsync(request.Id, cancellationToken);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("pricings-get", cancellationToken);

            return true;
        }
    }
}
