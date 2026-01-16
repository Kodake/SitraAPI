using SITRA.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace SITRA.Core.Mediator.Queries.Auths
{
    public class ValidateOpenedPettyCashQuery : IRequest<(bool Success, string? ErrorMessage)>
    {
    }
}
