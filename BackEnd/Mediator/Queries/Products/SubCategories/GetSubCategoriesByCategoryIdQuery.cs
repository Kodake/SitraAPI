using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.SubCategories
{
    public class GetSubCategoriesByCategoryIdQuery(long categoryId) : IQuery<List<SubCategoryListDTO>>
    {
        public long CategoryId { get; } = categoryId;

        public async Task<List<SubCategoryListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var subCategories = await unitOfWork.SubCategoryRepository.GetByCategoryId(CategoryId);
            var subCategoriesDTO = mapper.Map<List<SubCategoryListDTO>>(subCategories);
            return subCategoriesDTO;
        }
    }
}
