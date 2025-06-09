using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.SubCategories
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
