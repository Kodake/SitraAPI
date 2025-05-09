using AutoMapper;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Categories
{
    public class GetCategoryByIdQuery : GetByIdQuery<CategoryDTO>
    {
        public GetCategoryByIdQuery(long id) : base(id) { }

        public override async Task<CategoryDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(Id);
            return mapper.Map<CategoryDTO>(category);
        }
    }
}
