using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class UpdateFileCommand<TEntity, TDto>
        : IRequest<(byte[] FileBytes, string FileName)?>
        where TEntity : class
    {
        public Guid Id { get; set; }
        public TDto Dto { get; set; }

        protected UpdateFileCommand(Guid id, TDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public abstract Task<(byte[] FileBytes, string FileName)?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken);
    }
}
