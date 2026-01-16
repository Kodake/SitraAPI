using SITRA.Core.Mediator.Commands;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Handlers
{
    public class DeleteCommandHandler<TRequest> : IRequestHandler<TRequest, bool> where TRequest : DeleteCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;

        public DeleteCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
        }

        public async Task<bool> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, cancellationToken);
        }
    }
}
