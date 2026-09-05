using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Interfaces.Services.Warehouses;

namespace SITRA.Core.Mediator.Queries.Warehouses.Stocks
{
    public class GetAllStocksQuery : IRequest<List<StockListDTO>>
    {
        public PaginationDTO Pagination { get; set; }

        public GetAllStocksQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllStocksQueryHandler : IRequestHandler<GetAllStocksQuery, List<StockListDTO>>
    {
        private readonly IStockService _stockService;
        private readonly IMapper _mapper;

        public GetAllStocksQueryHandler(IStockService stockService, IMapper mapper)
        {
            _stockService = stockService;
            _mapper = mapper;
        }

        public async ValueTask<List<StockListDTO>> Handle(GetAllStocksQuery request, CancellationToken cancellationToken)
        {
            var stocks = await _stockService.GetPaginatedAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<StockListDTO>>(stocks);
        }
    }
}
