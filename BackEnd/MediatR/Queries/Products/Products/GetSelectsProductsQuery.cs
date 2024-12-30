using AutoMapper;
using BackEnd.DTOs.Products.Products;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Products
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
