using AutoMapper;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.SubCategories
{
    public class UpdateSubCategoryCommand : UpdateCommand<SubCategory, CreateSubCategoryDTO>
    {
        public UpdateSubCategoryCommand(long id, CreateSubCategoryDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var subCategoryDB = await unitOfWork.SubCategoryRepository.GetByIdAsync(Id);

            if (subCategoryDB == null) return false;

            mapper.Map(Dto, subCategoryDB);
            unitOfWork.SubCategoryRepository.Update(subCategoryDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("subCategories-get", default);

            return true;
        }
    }
}
