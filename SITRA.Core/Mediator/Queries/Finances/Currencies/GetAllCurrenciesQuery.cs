using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Currencies
{
    public class GetAllCurrenciesQuery : GetAllQuery<CurrencyListDTO>
    {
        public GetAllCurrenciesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<CurrencyListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var currencies = await unitOfWork.CurrencyRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<CurrencyListDTO>>(currencies);
        }
    }
}
