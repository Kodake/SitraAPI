using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Currencies
{
    public class GetAllCurrenciesQuery : GetAllQuery<CurrencyListDTO>
    {
        public GetAllCurrenciesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<CurrencyListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var currencies = await unitOfWork.CurrencyRepository.GetAllAsync(Pagination);
            return mapper.Map<List<CurrencyListDTO>>(currencies);
        }
    }
}
