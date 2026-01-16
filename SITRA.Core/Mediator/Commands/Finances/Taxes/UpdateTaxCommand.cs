using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Finances.Taxes
{
    public class UpdateTaxCommand : UpdateCommand<Tax, CreateTaxDTO>
    {
        public UpdateTaxCommand(long id, CreateTaxDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var taxDB = await unitOfWork.TaxRepository.GetByIdAsync(Id);

            if (taxDB == null) return false;

            mapper.Map(Dto, taxDB);
            unitOfWork.TaxRepository.Update(taxDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("taxes-get", default);

            return true;
        }
    }
}
