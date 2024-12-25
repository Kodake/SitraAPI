using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Brands;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Brands
{
    public class GetAllBrandsQuery : GetAllQuery<BrandListDTO>
    {
        public GetAllBrandsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<BrandListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var brands = await unitOfWork.BrandRepository.GetAllAsync(Pagination);
            return mapper.Map<List<BrandListDTO>>(brands);
        }
    }
}
