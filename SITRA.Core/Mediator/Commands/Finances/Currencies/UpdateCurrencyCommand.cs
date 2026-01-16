using AutoMapper;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Finances.Currencies
{
    public class UpdateCurrencyCommand : UpdateCommand<Currency, CreateCurrencyDTO>
    {
        public UpdateCurrencyCommand(long id, CreateCurrencyDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var currencyDB = await unitOfWork.CurrencyRepository.GetByIdAsync(Id);

            if (currencyDB == null) return false;

            mapper.Map(Dto, currencyDB);
            unitOfWork.CurrencyRepository.Update(currencyDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", default);

            return true;
        }
    }
}
