using SITRA.Core.Interfaces.Services.Finances;
using MediatR;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Enums;
using AutoMapper;

namespace SITRA.Core.Mediator.Queries.Finances.Currencies
{
    public class GetSelectsCurrenciesQuery : IRequest<List<CurrencyListDTO>>
    {
        public StatusType Status { get; set; }

        public GetSelectsCurrenciesQuery(StatusType status)
        {
            Status = status;
        }
    }

    public class GetSelectsCurrenciesQueryHandler : IRequestHandler<GetSelectsCurrenciesQuery, List<CurrencyListDTO>>
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;

        public GetSelectsCurrenciesQueryHandler(ICurrencyService currencyService, IMapper mapper)
        {
            _currencyService = currencyService;
            _mapper = mapper;
        }

        public async Task<List<CurrencyListDTO>> Handle(GetSelectsCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var currencies = await _currencyService.GetAllAsync(request.Status, cancellationToken);
            return _mapper.Map<List<CurrencyListDTO>>(currencies);
        }
    }
}
