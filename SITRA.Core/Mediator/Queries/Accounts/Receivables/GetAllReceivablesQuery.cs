using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Interfaces.Services.Accounts;

namespace SITRA.Core.Mediator.Queries.Receivables
{
    public class GetAllReceivablesQuery : IRequest<List<ReceivableListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllReceivablesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllReceivablesQueryHandler : IRequestHandler<GetAllReceivablesQuery, List<ReceivableListDTO>>
    {
        private readonly IReceivableService _receivableService;
        private readonly IMapper _mapper;

        public GetAllReceivablesQueryHandler(IReceivableService receivableService, IMapper mapper)
        {
            _receivableService = receivableService;
            _mapper = mapper;
        }

        public async ValueTask<List<ReceivableListDTO>> Handle(GetAllReceivablesQuery request, CancellationToken cancellationToken)
        {
            var receivables = await _receivableService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ReceivableListDTO>>(receivables);
        }
    }
}
