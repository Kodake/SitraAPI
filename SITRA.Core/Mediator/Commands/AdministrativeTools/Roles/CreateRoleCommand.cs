using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Roles
{
    public class CreateRoleCommand : CreateCommand<Role, CreateRoleDTO>
    {
        public CreateRoleCommand(CreateRoleDTO dto): base(dto) { }

        public override async Task<CreateRoleDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var role = mapper.Map<Role>(Dto);
            await unitOfWork.RoleRepository.CreateAsync(role);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("roles-get", cancellationToken);
            return Dto;
        }
    }
}
