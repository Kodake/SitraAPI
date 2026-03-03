using AutoMapper;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SITRA.Core.Mediator.Queries.Auths
{
    public class ValidateOpenedPettyCashQuery : AuthQuery<(bool Success, string? ErrorMessage)>
    {
        public override async Task<(bool Success, string? ErrorMessage)> ExecuteAction(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync();
            var userEmail = user?.Email?.Trim();

            if (string.IsNullOrWhiteSpace(userEmail))
                return (false, Utilities.UserNotAuthMessage);

            var openPettyCash = await unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            if (openPettyCash == null)
                return (false, Utilities.PettyCashNotOpenMessage);

            if (openPettyCash.User != null)
            {
                var openPettyCashUserEmail = openPettyCash.User.Email?.Trim();

                if (!string.IsNullOrWhiteSpace(openPettyCashUserEmail) &&
                    !string.Equals(openPettyCashUserEmail, userEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, string.Format(Utilities.PettyCashOpenedByOtherUserMessage, openPettyCashUserEmail));
                }
            }

            return (true, null);
        }
    }
}
