using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.SubCategories
{
    public class GetSelectsSubCategoriesQuery : GetSelectsQuery<SubCategoryListDTO>
    {
        public GetSelectsSubCategoriesQuery(StatusType status) : base(status) { }

        public override async Task<List<SubCategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var subCategories = await unitOfWork.SubCategoryRepository.GetAll(Status);
            return mapper.Map<List<SubCategoryListDTO>>(subCategories);
        }
    }
}
