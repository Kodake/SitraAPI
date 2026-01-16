using AutoMapper;
using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class UpdateFileCommand<TEntity, TDto>
        : IRequest<(byte[] FileBytes, string FileName)?>
        where TEntity : class
    {
        public long Id { get; set; }
        public TDto Dto { get; set; }

        protected UpdateFileCommand(long id, TDto dto)
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
