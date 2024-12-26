using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Entities.AdministrativeTools;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.AdministrativeTools.FiscalSupports
{
    public class UpdateFiscalSupportCommand : UpdateCommand<FiscalSupport, CreateFiscalSupportDTO>
    {
        public UpdateFiscalSupportCommand(int id, CreateFiscalSupportDTO dto) : base(id, dto) { }

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
