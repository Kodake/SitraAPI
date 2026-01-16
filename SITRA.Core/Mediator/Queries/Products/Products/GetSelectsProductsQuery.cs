using AutoMapper;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Products
{
    public class GetSelectsProductsQuery : GetSelectsQuery<ProductListDTO>
    {
        public GetSelectsProductsQuery(StatusType status) : base(status) { }

        public override async Task<List<ProductListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var products = await unitOfWork.ProductRepository.GetAll(Status);
            return mapper.Map<List<ProductListDTO>>(products);
        }
    }
}
