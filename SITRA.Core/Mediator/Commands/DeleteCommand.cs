using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class DeleteCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        protected DeleteCommand(Guid id)
        {
            Id = id;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken);
    }
}
