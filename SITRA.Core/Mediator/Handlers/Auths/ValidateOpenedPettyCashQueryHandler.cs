using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Queries.Auths;
using MediatR;
using Microsoft.AspNetCore.Http;

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
            var userEmail = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return (false, "No se pudo obtener el usuario desde el token.");
            }

            var openPettyCash = await _unitOfWork.PettyCashRepository.GetOpenPettyCashAsync();

            if (openPettyCash == null)
            {
                return (false, "Debe abrir la caja chica antes de continuar.");
            }

            if (openPettyCash.User != null &&
                !string.Equals(openPettyCash.User.Email?.Trim(), userEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return (false, $"El usuario {openPettyCash.User.Email} debe cerrar la caja chica previamente abierta.");
            }

            return (true, null);
        }
    }
}
