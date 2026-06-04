using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Products
{
    public class GetSelectsProductsQuery : IRequest<List<ProductListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsProductsQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsProductsQueryHandler : IRequestHandler<GetSelectsProductsQuery, List<ProductListDTO>>
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public GetSelectsProductsQueryHandler(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async ValueTask<List<ProductListDTO>> Handle(GetSelectsProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<ProductListDTO>>(products);
        }
    }
}
