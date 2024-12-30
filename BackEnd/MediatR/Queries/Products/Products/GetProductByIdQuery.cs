using AutoMapper;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Products.Products
{
    public class GetProductByIdQuery : GetByIdQuery<ProductDTO>
    {
        public GetProductByIdQuery(int id) : base(id) { }

        public override async Task<ProductDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(Id);
            return mapper.Map<ProductDTO>(product);
        }
    }
}