using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Products;
using BackEnd.Repositories;

namespace BackEnd.Mediator.Queries.Products.Products
{
    public class GetAllProductsQuery : GetAllQuery<ProductListDTO>
    {
        public GetAllProductsQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<ProductListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var products = await unitOfWork.ProductRepository.GetAllAsync(Pagination);
            return mapper.Map<List<ProductListDTO>>(products);
        }
    }
}
