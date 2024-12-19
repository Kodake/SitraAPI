using AutoMapper;
using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands
{
    public abstract class UpdateCommand<TEntity, TDto> : IRequest<bool>
        where TEntity : class
    {
        public int Id { get; set; }
        public TDto Dto { get; set; }

        protected UpdateCommand(int id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken);
    }
}
