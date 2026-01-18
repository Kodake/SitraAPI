using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries.Auths;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SITRA.Core.Mediator.Handlers.Auths
{
    public class ValidateOpenedPettyCashQueryHandler : IRequestHandler<ValidateOpenedPettyCashQuery, (bool Success, string? ErrorMessage)>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateOpenedPettyCashQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, string? ErrorMessage)> Handle(ValidateOpenedPettyCashQuery request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            
            // Retrieve email safely checking standard claim types
            var userEmail = user?.FindFirst(JwtRegisteredClaimNames.Email)?.Value 
                         ?? user?.FindFirst(ClaimTypes.Email)?.Value
                         ?? user?.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return (false, "No se pudo obtener el usuario desde el token.");
            }

            var openPettyCash = await _unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            if (openPettyCash == null)
            {
                return (false, "Debe abrir la caja chica antes de continuar.");
            }

            // Check if the open petty cash belongs to another user
            if (openPettyCash.User != null)
            {
                var openPettyCashUserEmail = openPettyCash.User.Email?.Trim();

                if (!string.IsNullOrWhiteSpace(openPettyCashUserEmail) &&
                    !string.Equals(openPettyCashUserEmail, userEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return (false, $"El usuario {openPettyCashUserEmail} debe cerrar la caja chica previamente abierta.");
                }
            }

            return (true, null);
        }
    }
}
