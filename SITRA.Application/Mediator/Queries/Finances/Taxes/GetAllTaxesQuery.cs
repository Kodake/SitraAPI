using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Interfaces.Services.Finances;

namespace SITRA.Core.Mediator.Queries.Finances.Taxes
{
    public class GetAllTaxesQuery : IRequest<List<TaxListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllTaxesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllTaxesQueryHandler : IRequestHandler<GetAllTaxesQuery, List<TaxListDTO>>
    {
        private readonly ITaxService _taxService;
        private readonly IMapper _mapper;

        public GetAllTaxesQueryHandler(ITaxService taxService, IMapper mapper)
        {
            _taxService = taxService;
            _mapper = mapper;
        }

        public async ValueTask<List<TaxListDTO>> Handle(GetAllTaxesQuery request, CancellationToken cancellationToken)
        {
            var taxes = await _taxService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<TaxListDTO>>(taxes);
        }
    }
}
