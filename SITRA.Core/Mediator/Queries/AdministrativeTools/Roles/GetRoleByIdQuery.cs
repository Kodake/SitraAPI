using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetRoleByIdQuery : GetByIdQuery<RoleDTO?>
    {
        public GetRoleByIdQuery(Guid id) : base(id) { }

        public override Task<RoleDTO?> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken) => throw new NotImplementedException("Use GetRoleByIdHandler");
    }

    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, RoleDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetRoleByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<RoleDTO?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(request.Id);
            if (role == null) return null;

            var dto = _mapper.Map<RoleDTO>(role);

            var identityRole = await _roleManager.FindByNameAsync(role.Name);
            if (identityRole != null)
            {
                var claims = await _roleManager.GetClaimsAsync(identityRole);
                dto.Permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
            }

            return dto;
        }
    }
}
