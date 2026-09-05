using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands
{
    public abstract class ToggleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        protected ToggleCommand(Guid id)
        {
            Id = id;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken);
    }
}
