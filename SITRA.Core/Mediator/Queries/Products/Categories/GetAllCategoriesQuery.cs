using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Categories
{
    public class GetAllCategoriesQuery : GetAllQuery<CategoryListDTO>
    {
        public GetAllCategoriesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<CategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var categories = await unitOfWork.CategoryRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<CategoryListDTO>>(categories);
        }
    }
}
