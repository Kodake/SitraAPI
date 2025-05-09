using AutoMapper;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Products.Brands
{
    public class CreateBrandCommand : CreateCommand<Brand, CreateBrandDTO>
    {
        public CreateBrandCommand(CreateBrandDTO dto) : base(dto) { }

        public override async Task<CreateBrandDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var brand = mapper.Map<Brand>(Dto);
            await unitOfWork.BrandRepository.CreateAsync(brand);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", cancellationToken);
            return Dto;
        }
    }
}
