using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Entities.Finances;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Finances.Taxes
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
