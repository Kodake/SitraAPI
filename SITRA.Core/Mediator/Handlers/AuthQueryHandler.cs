using AutoMapper;
using Mediator;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries;

namespace SITRA.Core.Mediator.Handlers
{
    public abstract class AuthQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : AuthQuery<TResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public ValueTask<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            return new ValueTask<TResult>(request.ExecuteAction(_unitOfWork, _mapper, cancellationToken));
        }
    }
}
