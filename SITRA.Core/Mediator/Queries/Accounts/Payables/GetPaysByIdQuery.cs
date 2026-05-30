using SITRA.Core.Interfaces.Services.Accounts;
using MediatR;
using SITRA.Core.DTOs.Accounts.Payables;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Accounts.Payables
{
    public class GetPaysByIdQuery : IRequest<PayableDTO>
    {
        public Guid Id { get; set; }

        public GetPaysByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPaysByIdQueryHandler : IRequestHandler<GetPaysByIdQuery, PayableDTO>
    {
        private readonly IPayableService _payableService;
        private readonly IMapper _mapper;

        public GetPaysByIdQueryHandler(IPayableService payableService, IMapper mapper)
        {
            _payableService = payableService;
            _mapper = mapper;
        }

        public async Task<PayableDTO> Handle(GetPaysByIdQuery request, CancellationToken cancellationToken)
        {
            var payable = await _payableService.GetPaysByIdAsync(request.Id, cancellationToken);
            return _mapper.Map<PayableDTO>(payable);
        }
    }
}
