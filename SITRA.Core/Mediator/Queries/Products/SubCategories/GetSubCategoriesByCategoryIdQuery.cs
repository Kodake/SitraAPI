using AutoMapper;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.SubCategories
{
    public class GetSubCategoriesByCategoryIdQuery(Guid categoryId) : IQuery<List<SubCategoryListDTO>>
    {
        public Guid CategoryId { get; } = categoryId;

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
