using AutoMapper;
using MediatR;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries;

namespace SITRA.Core.Mediator.Handlers
{
    public class AuthQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : AuthQuery<TResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
        {
            return request.ExecuteAction(_unitOfWork, _mapper, cancellationToken);
        }
    }
}
