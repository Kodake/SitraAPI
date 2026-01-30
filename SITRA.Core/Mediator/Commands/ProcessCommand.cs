using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class ProcessCommand<TEntity, TDto> : IRequest<TDto>
        where TEntity : class
    {
        public Guid Id { get; set; }
        public TDto Dto { get; set; }

        protected ProcessCommand(Guid id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<TDto> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
