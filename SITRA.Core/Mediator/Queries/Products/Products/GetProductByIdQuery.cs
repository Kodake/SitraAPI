using AutoMapper;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Products.Products
{
    public class GetProductByIdQuery : GetByIdQuery<ProductDTO>
    {
        public GetProductByIdQuery(Guid id) : base(id) { }

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
