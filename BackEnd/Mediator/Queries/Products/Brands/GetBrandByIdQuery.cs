using AutoMapper;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Brands
{
    public class GetBrandByIdQuery : GetByIdQuery<BrandDTO>
    {
        public GetBrandByIdQuery(long id) : base(id) { }

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