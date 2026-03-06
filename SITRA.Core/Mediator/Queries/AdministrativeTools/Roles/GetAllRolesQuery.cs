using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Queries.AdministrativeTools.Roles
{
    public class GetAllRolesQuery : GetAllQuery<RoleListDTO>
    {
        public GetAllRolesQuery(PaginationDTO pagination) : base(pagination) { }

        public override Task<List<RoleListDTO>> GetData(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CancellationToken cancellationToken) => throw new NotImplementedException("Use GetAllRolesHandler");
    }

    public class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, List<RoleListDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetAllRolesHandler(IUnitOfWork unitOfWork, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<List<RoleListDTO>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.RoleRepository.GetAllAsync(request.Pagination);
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
