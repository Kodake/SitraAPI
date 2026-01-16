using AutoMapper;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands
{
    public class ProcessCommandHandler<TEntity, TDto>
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

        public async Task<TDto> Handle(
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
