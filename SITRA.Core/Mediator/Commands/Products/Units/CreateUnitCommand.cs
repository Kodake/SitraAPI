using AutoMapper;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Entities.Products;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Products.Units
{
    public class CreateUnitCommand : CreateCommand<Unit, CreateUnitDTO>
    {
        public CreateUnitCommand(CreateUnitDTO dto) : base(dto) { }

        public override async Task<CreateUnitDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var unit = mapper.Map<Unit>(Dto);
            await unitOfWork.UnitRepository.CreateAsync(unit);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("units-get", cancellationToken);
            return Dto;
        }
    }
}
