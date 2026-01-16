using AutoMapper;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Units
{
    public class UpdateUnitCommand : UpdateCommand<Unit, CreateUnitDTO>
    {
        public UpdateUnitCommand(long id, CreateUnitDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(Id);

            if (unitDB == null) return false;

            mapper.Map(Dto, unitDB);
            unitOfWork.UnitRepository.Update(unitDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            return true;
        }
    }
}
