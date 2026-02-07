using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports
{
    public record SetDefaultFiscalSupportCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultFiscalSupportCommandHandler : IRequestHandler<SetDefaultFiscalSupportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultFiscalSupportCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(
            SetDefaultFiscalSupportCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.FiscalSupportRepository.SetDefault(request.Id);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync("fiscalsupports-get", cancellationToken);

            return true;
        }
    }
}
