using BackEnd.Repositories;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands
{
    public abstract class DeleteCommand : IRequest<bool>
    {
        public long Id { get; set; }

        protected DeleteCommand(long id)
        {
            Id = id;
        }

        public abstract Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken);
    }
}
