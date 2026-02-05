using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Finances.Currencies
{
    public record SetDefaultCurrencyCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultCurrencyCommandHandler : IRequestHandler<SetDefaultCurrencyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultCurrencyCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(
            SetDefaultCurrencyCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.CurrencyRepository.SetDefault(request.Id);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("currencies-get", cancellationToken);

            return true;
        }
    }
}
