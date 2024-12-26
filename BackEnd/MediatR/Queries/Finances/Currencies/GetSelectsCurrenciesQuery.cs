using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Enums;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Currencies
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
