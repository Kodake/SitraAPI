using AutoMapper;
using BackEnd.Repositories;
using MediatR;

namespace BackEnd.Mediator.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {
        Task<TResult> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken);
    }
}
