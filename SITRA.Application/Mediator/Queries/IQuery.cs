using AutoMapper;
using Mediator;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {
        Task<TResult> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken);
    }
}
