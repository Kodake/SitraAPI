using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetRoleByIdQuery : GetByIdQuery<RoleDTO>
    {
        public GetRoleByIdQuery(Guid id) : base(id) { }

        public override async Task<RoleDTO> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken)
        {
            var role = await unitOfWork.RoleRepository.GetByIdAsync(Id);
            return mapper.Map<RoleDTO>(role);
        }
    }
}
