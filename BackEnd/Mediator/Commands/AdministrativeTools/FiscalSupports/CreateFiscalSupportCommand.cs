using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.AdministrativeTools.FiscalSupports
{
    public class CreateFiscalSupportCommand : CreateCommand<FiscalSupport, CreateFiscalSupportDTO>
    {
        public CreateFiscalSupportCommand(CreateFiscalSupportDTO dto) : base(dto) { }

        public override async Task<CreateFiscalSupportDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var fiscalSupport = mapper.Map<FiscalSupport>(Dto);
            await unitOfWork.FiscalSupportRepository.CreateAsync(fiscalSupport);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscalsupports-get", cancellationToken);
            return Dto;
        }
    }
}
