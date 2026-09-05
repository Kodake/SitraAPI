using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.Interfaces;
using SITRA.Core.Interfaces.Services.Products;

namespace SITRA.Core.Mediator.Queries.Products.Brands
{
    public class GetAllBrandsQuery : IRequest<List<BrandListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllBrandsQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, List<BrandListDTO>>
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;

        public GetAllBrandsQueryHandler(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        public async ValueTask<List<BrandListDTO>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<BrandListDTO>>(brands);
        }
    }
}
