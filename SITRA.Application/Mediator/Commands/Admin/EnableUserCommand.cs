using Mediator;

namespace SITRA.Core.Mediator.Commands.Admin
{
    public record EnableUserCommand(string UserId) : IRequest<bool>;
}
