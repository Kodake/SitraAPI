using AutoMapper;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Finances.Currencies
{
    public class CreateCurrencyCommand : CreateCommand<Currency, CreateCurrencyDTO>
    {
        public CreateCurrencyCommand(CreateCurrencyDTO dto) : base(dto) { }

        public override async Task<CreateCurrencyDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var currency = mapper.Map<Currency>(Dto);
            await unitOfWork.CurrencyRepository.CreateAsync(currency);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("currencies-get", cancellationToken);
            return Dto;
        }
    }
}
