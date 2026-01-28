using AutoMapper;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Categories
{
    public class UpdateCategoryCommand : UpdateCommand<Category, CreateCategoryDTO>
    {
        public UpdateCategoryCommand(Guid id, CreateCategoryDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var categoryDB = await unitOfWork.CategoryRepository.GetByIdAsync(Id);

            if (categoryDB == null) return false;

            mapper.Map(Dto, categoryDB);
            unitOfWork.CategoryRepository.Update(categoryDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", default);

            return true;
        }
    }
}
