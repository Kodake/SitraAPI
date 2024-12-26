using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Finances.Currencies
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
