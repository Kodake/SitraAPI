using SITRA.Core.Interfaces.Services.Products;
using MediatR;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Categories
{
    public class GetSelectsCategoriesQuery : IRequest<List<CategoryListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsCategoriesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsCategoriesQueryHandler : IRequestHandler<GetSelectsCategoriesQuery, List<CategoryListDTO>>
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public GetSelectsCategoriesQueryHandler(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<List<CategoryListDTO>> Handle(GetSelectsCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<CategoryListDTO>>(categories);
        }
    }
}
