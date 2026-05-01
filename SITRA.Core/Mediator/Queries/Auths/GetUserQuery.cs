using AutoMapper;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.Auths
{
    public class GetUserQuery : AuthQuery<UserDTO?>
    {
        public override async Task<UserDTO?> ExecuteAction(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserService.GetUserWithRoleAsync(cancellationToken);

            if (user == null)
                return null;

            var dto = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                RucNumber = user.RucNumber!,
                Address = user.Address!,
                Province = user.Province!,
                Municipality = user.Municipality!,
                PhoneNumber = user.PhoneNumber!,
                CompanyName = user.CompanyName,
                ParentUserId = user.ParentUserId,
                RoleId = user.TenantRoleUsers.FirstOrDefault()?.RoleId.ToString()
            };

            return dto;
        }
    }
}
