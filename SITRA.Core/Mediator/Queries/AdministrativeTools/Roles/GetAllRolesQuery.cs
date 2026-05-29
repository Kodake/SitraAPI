using AutoMapper;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetAllRolesQuery : GetAllQuery<RoleListDTO>
    {
        public GetAllRolesQuery(PaginationDTO pagination) : base(pagination) { }

        public override async Task<List<RoleListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var roles = await unitOfWork.RoleRepository.GetReadOnlyAsync(Pagination);
            return mapper.Map<List<RoleListDTO>>(roles);
        }
    }
}
