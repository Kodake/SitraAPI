using AutoMapper;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Currencies
{
    public class GetSelectsCurrenciesQuery : GetSelectsQuery<CurrencyListDTO>
    {
        public GetSelectsCurrenciesQuery(StatusType status = StatusType.All) : base(status) { }

        public override async Task<List<CurrencyListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var currencies = await unitOfWork.CurrencyRepository.GetCurrencies();
            return mapper.Map<List<CurrencyListDTO>>(currencies);
        }
    }
}
