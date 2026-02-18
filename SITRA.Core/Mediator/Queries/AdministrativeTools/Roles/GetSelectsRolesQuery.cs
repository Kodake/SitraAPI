using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetSelectsRolesQuery : GetSelectsQuery<RoleListDTO>
    {
        public GetSelectsRolesQuery() : base() { }

        public override async Task<List<RoleListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var roles = await unitOfWork.RoleRepository.GetAll(cancellationToken);
            return mapper.Map<List<RoleListDTO>>(roles);
        }
    }
}
