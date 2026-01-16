using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports
{
    public class UpdateFiscalSupportCommand : UpdateCommand<FiscalSupport, CreateFiscalSupportDTO>
    {
        public UpdateFiscalSupportCommand(long id, CreateFiscalSupportDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var fiscalSupportDB = await unitOfWork.FiscalSupportRepository.GetByIdAsync(Id);

            if (fiscalSupportDB == null) return false;

            mapper.Map(Dto, fiscalSupportDB);
            unitOfWork.FiscalSupportRepository.Update(fiscalSupportDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("fiscalsupports-get", cancellationToken);

            return true;
        }
    }
}
