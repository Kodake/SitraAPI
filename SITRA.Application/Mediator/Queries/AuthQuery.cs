using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
{
    public abstract class AuthQuery<TResult> : IRequest<TResult>
    {
        public abstract Task<TResult> ExecuteAction(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken);
    }
}
