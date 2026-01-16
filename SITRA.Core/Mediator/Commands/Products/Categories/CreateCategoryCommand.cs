using AutoMapper;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Categories
{
    public class CreateCategoryCommand : CreateCommand<Category, CreateCategoryDTO>
    {
        public CreateCategoryCommand(CreateCategoryDTO dto) : base(dto) { }

        public override async Task<CreateCategoryDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var category = mapper.Map<Category>(Dto);
            await unitOfWork.CategoryRepository.CreateAsync(category);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("categories-get", cancellationToken);
            return Dto;
        }
    }
}
