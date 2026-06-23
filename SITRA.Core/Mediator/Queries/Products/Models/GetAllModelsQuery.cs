using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Models
{
    public class GetAllModelsQuery : IRequest<List<ModelListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllModelsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllModelsQueryHandler : IRequestHandler<GetAllModelsQuery, List<ModelListDTO>>
    {
        private readonly IModelService _modelService;
        private readonly IMapper _mapper;

        public GetAllModelsQueryHandler(IModelService modelService, IMapper mapper)
        {
            _modelService = modelService;
            _mapper = mapper;
        }

        public async ValueTask<List<ModelListDTO>> Handle(GetAllModelsQuery request, CancellationToken cancellationToken)
        {
            var models = await _modelService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ModelListDTO>>(models);
        }
    }
}
