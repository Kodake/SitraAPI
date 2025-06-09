using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.SubCategories
{
    public class GetSubCategoryByIdQuery : GetByIdQuery<SubCategoryDTO>
    {
        public GetSubCategoryByIdQuery(long id) : base(id) { }

        public override async Task<SubCategoryDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var subCategory = await unitOfWork.SubCategoryRepository.GetByIdAsync(Id);
            return mapper.Map<SubCategoryDTO>(subCategory);
        }
    }
}
