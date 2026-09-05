using AutoMapper;
using Mediator;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries;

public abstract class QueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : SITRA.Core.Mediator.Queries.IQuery<TResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async ValueTask<TResult> Handle(TQuery request, CancellationToken cancellationToken)
    {
        return await request.GetData(_unitOfWork, _mapper, cancellationToken);
    }
}
