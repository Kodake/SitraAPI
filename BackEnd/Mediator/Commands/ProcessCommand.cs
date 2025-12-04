using AutoMapper;
using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands
{
    public abstract class ProcessCommand<TEntity, TDto> : IRequest<TDto>
        where TEntity : class
    {
        public long Id { get; set; }
        public TDto Dto { get; set; }

        protected ProcessCommand(long id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<TDto> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
