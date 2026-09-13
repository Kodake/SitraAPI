using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Finances.Currencies
{
    public class UpdateCurrencyCommand : UpdateCommand<Currency, CreateCurrencyDTO>
    {
        public UpdateCurrencyCommand(Guid id, CreateCurrencyDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Id);

            if (currencyDB == null) return false;

            currencyDB.Symbol = Dto.Symbol;
            currencyDB.Rate = Dto.Rate;

            unitOfWork.CurrencyRepository.Update(currencyDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            return true;
        }
    }
}
