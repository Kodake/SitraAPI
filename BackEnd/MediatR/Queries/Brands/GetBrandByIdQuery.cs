using BackEnd.Repositories;
using BackEnd.DTOs.Products.Brands;
using AutoMapper;

namespace BackEnd.MediatR.Queries.Brands
{
    public class GetBrandByIdQuery : GetByIdQuery<BrandDTO>
    {
        public GetBrandByIdQuery(int id) : base(id) { }

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