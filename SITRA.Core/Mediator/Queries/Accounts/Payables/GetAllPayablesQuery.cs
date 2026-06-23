using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Interfaces.Services.Accounts;

namespace SITRA.Core.Mediator.Queries.Accounts.Payables
{
    public class GetAllPayablesQuery : IRequest<List<PayableListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllPayablesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllPayablesQueryHandler : IRequestHandler<GetAllPayablesQuery, List<PayableListDTO>>
    {
        private readonly IPayableService _payableService;
        private readonly IMapper _mapper;

        public GetAllPayablesQueryHandler(IPayableService payableService, IMapper mapper)
        {
            _payableService = payableService;
            _mapper = mapper;
        }

        public async ValueTask<List<PayableListDTO>> Handle(GetAllPayablesQuery request, CancellationToken cancellationToken)
        {
            var payables = await _payableService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<PayableListDTO>>(payables);
        }
    }
}
