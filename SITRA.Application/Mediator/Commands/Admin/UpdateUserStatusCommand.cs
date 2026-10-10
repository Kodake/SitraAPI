using Mediator;
using SITRA.Core.DTOs.Admin;

namespace SITRA.Core.Mediator.Commands.Admin
{
    public record UpdateUserStatusCommand(string UserId, UpdateUserStatusDTO Dto) : IRequest<bool>;
}
