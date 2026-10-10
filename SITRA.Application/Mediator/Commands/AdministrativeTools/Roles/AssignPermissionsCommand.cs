using Mediator;
using SITRA.Core.DTOs.Auths;

namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Roles
{
    public record AssignPermissionsCommand(AssignPermissionsDTO DTO) : IRequest<bool>;
}
