using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Contacts
{
    public record SetDefaultContactCommand(Guid Id) : IRequest<bool>;

    public class SetDefaultContactCommandHandler : IRequestHandler<SetDefaultContactCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public SetDefaultContactCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(
            SetDefaultContactCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ContactRepository.SetDefault(request.Id, cancellationToken);

            if (!result)
                return false;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _outputCacheStore.EvictByTagAsync("contacts-get", cancellationToken);

            return true;
        }
    }
}
