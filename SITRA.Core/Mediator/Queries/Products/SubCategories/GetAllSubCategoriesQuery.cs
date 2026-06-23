using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.SubCategories
{
    public class GetAllSubCategoriesQuery : IRequest<List<SubCategoryListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllSubCategoriesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllSubCategoriesQueryHandler : IRequestHandler<GetAllSubCategoriesQuery, List<SubCategoryListDTO>>
    {
        private readonly ISubCategoryService _subCategoryService;
        private readonly IMapper _mapper;

        public GetAllSubCategoriesQueryHandler(ISubCategoryService subCategoryService, IMapper mapper)
        {
            _subCategoryService = subCategoryService;
            _mapper = mapper;
        }

        public async ValueTask<List<SubCategoryListDTO>> Handle(GetAllSubCategoriesQuery request, CancellationToken cancellationToken)
        {
            var subCategories = await _subCategoryService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<SubCategoryListDTO>>(subCategories);
        }
    }
}
