using AutoMapper;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Finances.Currencies
{
    public class GetCurrencyByIdQuery : GetByIdQuery<CurrencyDTO>
    {
        public GetCurrencyByIdQuery(Guid id) : base(id) { }

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
