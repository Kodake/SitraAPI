using SITRA.Core.Interfaces.Services.Finances;
using MediatR;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Finances.Taxes
{
    public class GetSelectsTaxesQuery : IRequest<List<TaxListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsTaxesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsTaxesQueryHandler : IRequestHandler<GetSelectsTaxesQuery, List<TaxListDTO>>
    {
        private readonly ITaxService _taxService;
        private readonly IMapper _mapper;

        public GetSelectsTaxesQueryHandler(ITaxService taxService, IMapper mapper)
        {
            _taxService = taxService;
            _mapper = mapper;
        }

        public async Task<List<TaxListDTO>> Handle(GetSelectsTaxesQuery request, CancellationToken cancellationToken)
        {
            var taxes = await _taxService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<TaxListDTO>>(taxes);
        }
    }
}
