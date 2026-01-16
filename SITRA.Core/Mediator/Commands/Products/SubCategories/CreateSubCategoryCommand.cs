using AutoMapper;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.SubCategories
{
    public class CreateSubCategoryCommand : CreateCommand<SubCategory, CreateSubCategoryDTO>
    {
        public CreateSubCategoryCommand(CreateSubCategoryDTO dto) : base(dto) { }

        public override async Task<CreateSubCategoryDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var subCategory = mapper.Map<SubCategory>(Dto);
            await unitOfWork.SubCategoryRepository.CreateAsync(subCategory);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("subCategories-get", cancellationToken);
            return Dto;
        }
    }
}
