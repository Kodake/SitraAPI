using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands;

namespace SITRA.Core.Mediator.Handlers
{
    public abstract class DeleteCommandHandler<TRequest> : IRequestHandler<TRequest, bool> where TRequest : DeleteCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public DeleteCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async ValueTask<bool> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, cancellationToken);
        }
    }
}
