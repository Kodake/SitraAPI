using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Combos
{
    public class GetStockCombosQuery : IRequest<List<ComboListDTO>>
    {
    }

    public class GetStockCombosQueryHandler : IRequestHandler<GetStockCombosQuery, List<ComboListDTO>>
    {
        private readonly IComboService _comboService;
        private readonly IMapper _mapper;

        public GetStockCombosQueryHandler(IComboService comboService, IMapper mapper)
        {
            _comboService = comboService;
            _mapper = mapper;
        }

        public async ValueTask<List<ComboListDTO>> Handle(GetStockCombosQuery request, CancellationToken cancellationToken)
        {
            var combos = await _comboService.GetStockCombosAsync(cancellationToken);
            return _mapper.Map<List<ComboListDTO>>(combos);
        }
    }
}
