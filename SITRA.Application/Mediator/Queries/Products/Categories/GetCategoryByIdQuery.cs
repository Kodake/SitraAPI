using AutoMapper;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Categories
{
    public class GetCategoryByIdQuery : GetByIdQuery<CategoryDTO>
    {
        public GetCategoryByIdQuery(Guid id) : base(id) { }

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
