using AutoMapper;
using BackEnd.DTOs.Products.Units;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Units
{
    public class UpdateUnitCommand : UpdateCommand<Unit, CreateUnitDTO>
    {
        public UpdateUnitCommand(int id, CreateUnitDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var unitDB = await unitOfWork.UnitRepository.GetByIdAsync(Id);

            if (unitDB == null) return false;

            unitDB.Name = Dto.Name;
            unitDB.ShortName = Dto.ShortName;
            unitOfWork.UnitRepository.Update(unitDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", default);

            return true;
        }
    }
}
