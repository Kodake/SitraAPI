using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Combos
{
    public class GetAllCombosQuery : IRequest<List<ComboListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllCombosQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllCombosQueryHandler : IRequestHandler<GetAllCombosQuery, List<ComboListDTO>>
    {
        private readonly IComboService _comboService;
        private readonly IMapper _mapper;

        public GetAllCombosQueryHandler(IComboService comboService, IMapper mapper)
        {
            _comboService = comboService;
            _mapper = mapper;
        }

        public async ValueTask<List<ComboListDTO>> Handle(GetAllCombosQuery request, CancellationToken cancellationToken)
        {
            var combos = await _comboService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ComboListDTO>>(combos);
        }
    }
}
