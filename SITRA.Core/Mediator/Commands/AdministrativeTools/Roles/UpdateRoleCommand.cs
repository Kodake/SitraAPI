using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Roles
{
    public class UpdateRoleCommand : UpdateCommand<Role, CreateRoleDTO>
    {
        public UpdateRoleCommand(Guid id, CreateRoleDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var roleDB = await unitOfWork.RoleRepository.GetByIdAsync(Id);

            if (roleDB == null) return false;

            mapper.Map(Dto, roleDB);
            unitOfWork.RoleRepository.Update(roleDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("roles-get", default);

            return true;
        }
    }
}
