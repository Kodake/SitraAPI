using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Auths;

namespace SITRA.Core.Mediator.Queries.Tenants
{
    public class GetAllTenantUsersQuery : GetAllQuery<UserDTO>
    {
        public GetAllTenantUsersQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<UserDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var users = await unitOfWork.UserService.GetAllAssociatedUsersAsync(Pagination, cancellationToken);
            return mapper.Map<List<UserDTO>>(users);
        }
    }
}
