using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Repositories;

namespace BackEnd.MediatR.Queries.Finances.Currencies
{
    public class GetCurrencyByIdQuery : GetByIdQuery<CurrencyDTO>
    {
        public GetCurrencyByIdQuery(int id) : base(id) { }

        public override async Task<CurrencyDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var currency = await unitOfWork.CurrencyRepository.GetByIdAsync(Id);
            return mapper.Map<CurrencyDTO>(currency);
        }
    }
}
