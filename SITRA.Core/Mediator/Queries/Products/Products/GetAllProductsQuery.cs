using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Products
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
