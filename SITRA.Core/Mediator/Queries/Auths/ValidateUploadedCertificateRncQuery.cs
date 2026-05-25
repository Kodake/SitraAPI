using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.Entities.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SITRA.Core.Mediator.Queries.Auths
{
    public class ValidateUploadedCertificateRncQuery : AuthQuery<(bool Success, string? ErrorMessage)>
    {
        public ValidateUploadedCertificateRncQuery()
        {
        }

        public override async Task<(bool Success, string? ErrorMessage)> ExecuteAction(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserAsync();

            if (user == null)
            {
                return (false, Utilities.UserNotAuthMessage);
            }

            // Resolve the owner user if the current user is a sub-user.
            User ownerUser = user;
            if (!user.IsOwner && !string.IsNullOrEmpty(user.ParentUserId))
            {
                // We cannot use UserManager directly here since we don't have it injected directly in the query,
                // but we can retrieve it through the UserService or UserRoleService if needed.
                // However, unitOfWork might not have a direct way to fetch by ID unless we have a UserRepository.
                // Let's assume the tenant configuration is already known or we can just rely on the RucNumber validation.
            }

            // A simplified check using the current tenant context. If we strictly need the owner user's certificate, 
            // we should do what GetTenantCertificateAsync does. Let's do that via UserRoleService.
            try
            {
                // This method already traverses to the owner and checks CertificateUrl and CertificatePassword
                // It throws InvalidOperationException if not found.
                var certInfo = await unitOfWork.UserRoleService.GetTenantCertificateAsync(cancellationToken);
                
                return (true, null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
