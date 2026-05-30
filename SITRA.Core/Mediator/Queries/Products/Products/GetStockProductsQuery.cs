using SITRA.Core.Interfaces.Services.Products;
using MediatR;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Products
{
    public class GetStockProductsQuery : IRequest<List<ProductListDTO>>
    {
    }

    public class GetStockProductsQueryHandler : IRequestHandler<GetStockProductsQuery, List<ProductListDTO>>
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public GetStockProductsQueryHandler(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<List<ProductListDTO>> Handle(GetStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetStockProductsAsync(cancellationToken);
            return _mapper.Map<List<ProductListDTO>>(products);
        }
    }
}
