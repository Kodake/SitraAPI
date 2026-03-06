using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetSelectsRolesQuery : GetSelectsQuery<RoleListDTO>
    {
        public GetSelectsRolesQuery() : base() { }

        public override Task<List<RoleListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken) => throw new NotImplementedException("Use GetSelectsRolesHandler");
    }

    public class GetSelectsRolesHandler : IRequestHandler<GetSelectsRolesQuery, List<RoleListDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetSelectsRolesHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<List<RoleListDTO>> Handle(GetSelectsRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.RoleRepository.GetAll(cancellationToken);
            var dtos = _mapper.Map<List<RoleListDTO>>(roles);

            foreach (var dto in dtos)
            {
                var identityRole = await _roleManager.FindByNameAsync(dto.Name);
                if (identityRole != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(identityRole);
                    dto.Permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
                }
            }

            return dtos;
        }
    }
}
