using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Products.Brands
{
    public class UpdateBrandCommand : UpdateCommand<Brand, CreateBrandDTO>
    {
        public UpdateBrandCommand(Guid id, CreateBrandDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var brandDB = await unitOfWork.BrandRepository.GetByIdAsync(Id);

            if (brandDB == null) return false;

            mapper.Map(Dto, brandDB);
            unitOfWork.BrandRepository.Update(brandDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            return true;
        }
    }
}
