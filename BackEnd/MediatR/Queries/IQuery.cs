using AutoMapper;
using BackEnd.Repositories;
using MediatR;

namespace BackEnd.MediatR.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {
        Task<TResult> GetData(IUnitOfWork unitOfWork, IMapper mapper, CancellationToken cancellationToken);
    }
}
