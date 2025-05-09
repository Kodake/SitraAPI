using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Finances.Taxes
{
    public class CreateTaxCommand : CreateCommand<Tax, CreateTaxDTO>
    {
        public CreateTaxCommand(CreateTaxDTO dto) : base(dto) { }

        public override async Task<CreateTaxDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var tax = mapper.Map<Tax>(Dto);
            await unitOfWork.TaxRepository.CreateAsync(tax);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", cancellationToken);
            return Dto;
        }
    }
}
