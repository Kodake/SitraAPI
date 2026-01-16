using AutoMapper;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.SubCategories
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
