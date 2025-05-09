using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Categories
{
    public class GetAllCategoriesQuery : GetAllQuery<CategoryListDTO>
    {
        public GetAllCategoriesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<CategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var categories = await unitOfWork.CategoryRepository.GetAllAsync(Pagination);
            return mapper.Map<List<CategoryListDTO>>(categories);
        }
    }
}
