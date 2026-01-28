using AutoMapper;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Brands
{
    public class GetBrandByIdQuery : GetByIdQuery<BrandDTO>
    {
        public GetBrandByIdQuery(Guid id) : base(id) { }

        public override async Task<BrandDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var brand = await unitOfWork.BrandRepository.GetByIdAsync(Id);
            return mapper.Map<BrandDTO>(brand);
        }
    }
}
