using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class ToggleCommand : IRequest<bool>
    {
        public long Id { get; set; }

        protected ToggleCommand(long id)
        {
            Id = id;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken);
    }
}
