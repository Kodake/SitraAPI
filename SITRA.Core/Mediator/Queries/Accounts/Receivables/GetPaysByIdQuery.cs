using SITRA.Core.Interfaces.Services.Accounts;
using MediatR;
using SITRA.Core.DTOs.Accounts.Receivables;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Accounts.Receivables
{
    public class GetPaysByIdQuery : IRequest<ReceivableDTO>
    {
        public Guid Id { get; set; }

        public GetPaysByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPaysByIdQueryHandler : IRequestHandler<GetPaysByIdQuery, ReceivableDTO>
    {
        private readonly IReceivableService _receivableService;
        private readonly IMapper _mapper;

        public GetPaysByIdQueryHandler(IReceivableService receivableService, IMapper mapper)
        {
            _receivableService = receivableService;
            _mapper = mapper;
        }

        public async Task<ReceivableDTO> Handle(GetPaysByIdQuery request, CancellationToken cancellationToken)
        {
            var receivable = await _receivableService.GetPaysByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<ReceivableDTO>(receivable);
        }
    }
}
