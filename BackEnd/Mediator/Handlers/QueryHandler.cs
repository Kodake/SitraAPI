using AutoMapper;
using BackEnd.Mediator.Queries;
using BackEnd.Repositories;
using MediatR;

public class QueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
    {
        return await request.GetData(_unitOfWork, _mapper, cancellationToken);
    }
}
