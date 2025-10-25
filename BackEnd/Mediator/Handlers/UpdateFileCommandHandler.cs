using AutoMapper;
using BackEnd.Mediator.Commands;
using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Handlers
{
    public class UpdateFileCommandHandler<TEntity, TDto>
        : IRequestHandler<UpdateFileCommand<TEntity, TDto>, (byte[] FileBytes, string FileName)?>
        where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IMapper _mapper;

        public UpdateFileCommandHandler(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _outputCacheStore = outputCacheStore;
            _mapper = mapper;
        }

        public async Task<(byte[] FileBytes, string FileName)?> Handle(
            UpdateFileCommand<TEntity, TDto> request,
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
