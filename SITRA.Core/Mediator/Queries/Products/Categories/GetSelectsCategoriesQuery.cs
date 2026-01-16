using AutoMapper;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Categories
{
    public class GetSelectsCategoriesQuery : GetSelectsQuery<CategoryListDTO>
    {
        public GetSelectsCategoriesQuery(StatusType status) : base(status) { }

        public override async Task<List<CategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var categories = await unitOfWork.CategoryRepository.GetAll(Status);
            return mapper.Map<List<CategoryListDTO>>(categories);
        }
    }
}
