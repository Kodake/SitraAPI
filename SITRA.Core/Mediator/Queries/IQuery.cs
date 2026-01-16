using AutoMapper;
using SITRA.Core.Interfaces;
using MediatR;

namespace SITRA.Core.Mediator.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {
        Task<TResult> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken);
    }
}
