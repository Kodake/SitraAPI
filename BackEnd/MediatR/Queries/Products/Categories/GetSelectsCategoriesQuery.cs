using AutoMapper;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Categories
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
