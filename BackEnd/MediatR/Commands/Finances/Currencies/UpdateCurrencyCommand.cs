using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Finances.Currencies
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
