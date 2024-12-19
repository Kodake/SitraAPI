using AutoMapper;
using BackEnd.MediatR.Commands;
using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Handlers
{
    public class UpdateCommandHandler<TEntity, TDto> : IRequestHandler<UpdateCommand<TEntity, TDto>, bool>
        where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public UpdateCommandHandler(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateCommand<TEntity, TDto> request, CancellationToken cancellationToken)
        {
            return await request.ExecuteAction(_unitOfWork, _outputCacheStore, _mapper, cancellationToken);
        }
    }
}
