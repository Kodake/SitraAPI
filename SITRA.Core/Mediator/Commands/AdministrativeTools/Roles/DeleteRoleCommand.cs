using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Roles
{
    public class DeleteRoleCommand : DeleteCommand
    {
        public DeleteRoleCommand(Guid id) : base(id) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, CancellationToken cancellationToken)
        {
            var role = await unitOfWork.RoleRepository.GetByIdAsync(Id, cancellationToken);
            if (role == null)
            {
                throw new KeyNotFoundException(string.Format(Utilities.RoleNotFoundMessage, Id));
            }

            if (role.IsSystemRole)
            {
               throw new InvalidOperationException(Utilities.CannotDeleteSystemRoleMessage);
            }

            await unitOfWork.RoleRepository.DeleteAsync(Id, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await outputCacheStore.EvictByTagAsync("roles-get", cancellationToken);

            return true;
        }
    }
}
