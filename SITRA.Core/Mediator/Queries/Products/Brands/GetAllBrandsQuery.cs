using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Brands
{
    public class GetAllBrandsQuery : GetAllQuery<BrandListDTO>
    {
        public GetAllBrandsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<BrandListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var brands = await unitOfWork.BrandRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<BrandListDTO>>(brands);
        }
    }
}
