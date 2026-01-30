using MediatR;

namespace SITRA.Core.Mediator.Queries.Auths
{
    public class ValidateOpenedPettyCashQuery : IRequest<(bool Success, string? ErrorMessage)>
    {
    }
}
