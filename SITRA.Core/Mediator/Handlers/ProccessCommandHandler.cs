using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class ProcessCommandHandler<TEntity, TDto>
        : IRequestHandler<ProcessCommand<TEntity, TDto>, TDto>
        where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public ProcessCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async ValueTask<TDto> Handle(
            ProcessCommand<TEntity, TDto> request,
            CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(
                _unitOfWork,
                _outputCacheStore,
                _mapper,
                cancellationToken);
        }
    }
}
