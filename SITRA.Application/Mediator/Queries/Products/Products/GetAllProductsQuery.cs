using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Products
{
    public class GetAllProductsQuery : IRequest<List<ProductListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllProductsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductListDTO>>
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async ValueTask<List<ProductListDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<ProductListDTO>>(products);
        }
    }
}
