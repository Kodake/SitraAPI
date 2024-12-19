using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands
{
    public abstract class ToggleCommand : IRequest<bool>
    {
        public int Id { get; set; }

        protected ToggleCommand(int id)
        {
            Id = id;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken);
    }
}
