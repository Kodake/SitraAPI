using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.SubCategories
{
    public class GetAllSubCategoriesQuery : GetAllQuery<SubCategoryListDTO>
    {
        public GetAllSubCategoriesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SubCategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var subCategories = await unitOfWork.SubCategoryRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<SubCategoryListDTO>>(subCategories);
        }
    }
}
