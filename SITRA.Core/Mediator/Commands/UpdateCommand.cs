using AutoMapper;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class UpdateCommand<TEntity, TDto> : IRequest<bool>
        where TEntity : class
    {
        public long Id { get; set; }
        public TDto Dto { get; set; }

        protected UpdateCommand(long id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
