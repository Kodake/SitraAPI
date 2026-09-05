using SITRA.Core.DTOs;
using SITRA.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading;
using System.Threading.Tasks;

namespace SITRA.Core.Mediator.Commands.Auths
{
    public class RefreshTokenCommand : AuthCommand<TokenResponseDTO?>
    {
        public string ExpiredToken { get; }
        public string RefreshToken { get; }

        public RefreshTokenCommand(string expiredToken, string refreshToken)
        {
            ExpiredToken = expiredToken;
            RefreshToken = refreshToken;
        }

        public override Task<TokenResponseDTO?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            return unitOfWork.AuthenticationService.RefreshTokenAsync(ExpiredToken, RefreshToken, cancellationToken);
        }
    }
}
