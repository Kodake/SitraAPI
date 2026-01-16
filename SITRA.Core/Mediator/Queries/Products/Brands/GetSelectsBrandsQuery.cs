using AutoMapper;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Brands
{
    public class GetSelectsBrandsQuery : GetSelectsQuery<BrandListDTO>
    {
        public GetSelectsBrandsQuery(StatusType status) : base(status) { }

        public override async Task<List<BrandListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var brands = await unitOfWork.BrandRepository.GetAll(Status);
            return mapper.Map<List<BrandListDTO>>(brands);
        }
    }
}
