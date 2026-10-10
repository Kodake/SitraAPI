using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Categories
{
    public class GetAllCategoriesQuery : IRequest<List<CategoryListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllCategoriesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryListDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async ValueTask<List<CategoryListDTO>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<CategoryListDTO>>(categories);
        }
    }
}
