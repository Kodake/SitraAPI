using AutoMapper;
using MediatR;

namespace SITRA.Core.Mediator.Queries
{
    public interface IQueryMediator<TResult> : IRequest<TResult>
    {
        Task<TResult> GetData(IMediator mediator, IMapper mapper, CancellationToken cancellationToken);
    }
}
