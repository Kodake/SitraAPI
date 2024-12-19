using AutoMapper;
using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands
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
