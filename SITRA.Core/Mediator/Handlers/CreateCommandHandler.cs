using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands;

namespace SITRA.Core.Mediator.Handlers
{
    public class CreateCommandHandler<TEntity, TDto> : IRequestHandler<CreateCommand<TEntity, TDto>, TDto>
        where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async Task<TDto> Handle(CreateCommand<TEntity, TDto> request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
        }
    }
}
