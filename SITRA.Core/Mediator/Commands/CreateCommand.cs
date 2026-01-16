using AutoMapper;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class CreateCommand<TEntity, TDto> : IRequest<TDto>
        where TEntity : class
    {
        public TDto Dto { get; set; }

        protected CreateCommand(TDto dto)
        {
            Dto = dto;
        }

        public abstract Task<TDto> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
