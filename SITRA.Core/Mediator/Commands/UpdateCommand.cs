using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class UpdateCommand<TEntity, TDto> : IRequest<bool>
        where TEntity : class
    {
        public Guid Id { get; set; }
        public TDto Dto { get; set; }

        protected UpdateCommand(Guid id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
