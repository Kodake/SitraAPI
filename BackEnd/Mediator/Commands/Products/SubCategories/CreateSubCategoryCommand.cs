using AutoMapper;
using BackEnd.DTOs.Products.SubCategories;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.SubCategories
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
