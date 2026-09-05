using AutoMapper;
using Mediator;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Interfaces.Services.Finances;

namespace SITRA.Core.Mediator.Queries.Finances.Currencies
{
    public class GetAllCurrenciesQuery : IRequest<List<CurrencyListDTO>>
    {
        public PaginationDTO Pagination { get; set; }
        public GetAllCurrenciesQuery(PaginationDTO pagination)
        {
            Pagination = pagination;
        }
    }

    public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, List<CurrencyListDTO>>
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;

        public GetAllCurrenciesQueryHandler(ICurrencyService currencyService, IMapper mapper)
        {
            _currencyService = currencyService;
            _mapper = mapper;
        }

        public async ValueTask<List<CurrencyListDTO>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var currencies = await _currencyService.GetAllAsync(request.Pagination, cancellationToken);
            return _mapper.Map<List<CurrencyListDTO>>(currencies);
        }
    }
}
