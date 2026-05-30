using SITRA.Core.Interfaces.Services.AdministrativeTools;
using MediatR;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Deadlines
{
    public class GetSelectsDeadlinesQuery : IRequest<List<DeadlineListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsDeadlinesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsDeadlinesQueryHandler : IRequestHandler<GetSelectsDeadlinesQuery, List<DeadlineListDTO>>
    {
        private readonly IDeadlineService _service;
        private readonly IMapper _mapper;

        public GetSelectsDeadlinesQueryHandler(IDeadlineService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<List<DeadlineListDTO>> Handle(GetSelectsDeadlinesQuery request, CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<DeadlineListDTO>>(items);
        }
    }
}
