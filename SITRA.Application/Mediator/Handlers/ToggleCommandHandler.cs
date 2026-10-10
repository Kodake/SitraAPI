using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands;

namespace SITRA.Core.Mediator.Handlers
{
    public abstract class ToggleCommandHandler<TCommand> : IRequestHandler<TCommand, bool>
        where TCommand : ToggleCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public ToggleCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async ValueTask<bool> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, cancellationToken);
        }
    }
}
