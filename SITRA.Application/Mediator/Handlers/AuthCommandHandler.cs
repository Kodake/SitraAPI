using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands;

namespace SITRA.Core.Mediator.Handlers
{
    public abstract class AuthCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : AuthCommand<TResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public AuthCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public ValueTask<TResult> Handle(TCommand request, CancellationToken cancellationToken)
        {
            return new ValueTask<TResult>(request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken));
        }
    }
}
