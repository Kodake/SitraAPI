using AutoMapper;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Brands
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
