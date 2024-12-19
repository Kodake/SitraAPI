using AutoMapper;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Brands
{
    public class UpdateBrandCommand : UpdateCommand<Brand, CreateBrandDTO>
    {
        public UpdateBrandCommand(int id, CreateBrandDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var brandDB = await unitOfWork.BrandRepository.GetByIdAsync(Id);

            if (brandDB == null) return false;

            brandDB.Name = Dto.Name;
            unitOfWork.BrandRepository.Update(brandDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("brands-get", default);

            return true;
        }
    }
}
