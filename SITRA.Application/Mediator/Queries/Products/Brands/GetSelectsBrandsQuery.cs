using SITRA.Core.Interfaces.Services.Products;
using Mediator;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Products.Brands
{
    public class GetSelectsBrandsQuery : IRequest<List<BrandListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsBrandsQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsBrandsQueryHandler : IRequestHandler<GetSelectsBrandsQuery, List<BrandListDTO>>
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;

        public GetSelectsBrandsQueryHandler(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        public async ValueTask<List<BrandListDTO>> Handle(GetSelectsBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<BrandListDTO>>(brands);
        }
    }
}
