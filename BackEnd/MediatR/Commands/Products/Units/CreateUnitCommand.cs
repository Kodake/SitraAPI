using AutoMapper;
using BackEnd.DTOs.Products.Units;
using BackEnd.Entities.Products;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Products.Units
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
