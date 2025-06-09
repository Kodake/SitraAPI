using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.SubCategories
{
    public class GetAllSubCategoriesQuery : GetAllQuery<SubCategoryListDTO>
    {
        public GetAllSubCategoriesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<SubCategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var subCategories = await unitOfWork.SubCategoryRepository.GetAllAsync(Pagination);
            return mapper.Map<List<SubCategoryListDTO>>(subCategories);
        }
    }
}
