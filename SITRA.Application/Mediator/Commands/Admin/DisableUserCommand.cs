using Mediator;

namespace SITRA.Core.Mediator.Commands.Admin
{
    public record DisableUserCommand(string UserId, string? Reason = null) : IRequest<bool>;
}
