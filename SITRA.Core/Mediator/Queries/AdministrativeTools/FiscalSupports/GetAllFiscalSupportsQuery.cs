using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Interfaces.Services.AdministrativeTools;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetAllFiscalSupportsQuery : IRequest<List<FiscalSupportListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllFiscalSupportsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllFiscalSupportsQueryHandler : IRequestHandler<GetAllFiscalSupportsQuery, List<FiscalSupportListDTO>>
    {
        private readonly IFiscalSupportService _fiscalSupportService;
        private readonly IMapper _mapper;

        public GetAllFiscalSupportsQueryHandler(IFiscalSupportService fiscalSupportService, IMapper mapper)
        {
            _fiscalSupportService = fiscalSupportService;
            _mapper = mapper;
        }

        public async ValueTask<List<FiscalSupportListDTO>> Handle(GetAllFiscalSupportsQuery request, CancellationToken cancellationToken)
        {
            var fiscalSupports = await _fiscalSupportService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<FiscalSupportListDTO>>(fiscalSupports);
        }
    }
}
