using SITRA.Core.Interfaces.Services.AdministrativeTools;
using MediatR;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports
{
    public class GetSelectsFiscalSupportsQuery : IRequest<List<FiscalSupportListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsFiscalSupportsQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsFiscalSupportsQueryHandler : IRequestHandler<GetSelectsFiscalSupportsQuery, List<FiscalSupportListDTO>>
    {
        private readonly IFiscalSupportService _service;
        private readonly IMapper _mapper;

        public GetSelectsFiscalSupportsQueryHandler(IFiscalSupportService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<List<FiscalSupportListDTO>> Handle(GetSelectsFiscalSupportsQuery request, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<FiscalSupportListDTO>>(items);
        }
    }
}
