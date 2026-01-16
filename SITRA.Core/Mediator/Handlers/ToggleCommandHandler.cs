using SITRA.Core.Mediator.Commands;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Handlers
{
    public class ToggleCommandHandler<TCommand> : IRequestHandler<TCommand, bool>
        where TCommand : ToggleCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public ToggleCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, cancellationToken);
        }
    }
}
