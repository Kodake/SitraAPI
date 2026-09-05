using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Combos
{
    public class GetSelectsCombosQuery : IRequest<List<ComboListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsCombosQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsCombosQueryHandler : IRequestHandler<GetSelectsCombosQuery, List<ComboListDTO>>
    {
        private readonly IComboService _comboService;
        private readonly IMapper _mapper;

        public GetSelectsCombosQueryHandler(IComboService comboService, IMapper mapper)
        {
            _comboService = comboService;
            _mapper = mapper;
        }

        public async ValueTask<List<ComboListDTO>> Handle(GetSelectsCombosQuery request, CancellationToken cancellationToken)
        {
            var combos = await _comboService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<ComboListDTO>>(combos);
        }
    }
}
