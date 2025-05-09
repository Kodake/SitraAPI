using AutoMapper;
using BackEnd.DTOs.Products.Categories;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Categories
{
    public class UpdateCategoryCommand : UpdateCommand<Category, CreateCategoryDTO>
    {
        public UpdateCategoryCommand(long id, CreateCategoryDTO dto) : base(id, dto) { }

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
