using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.API.Profiles.AdministrativeTools
{
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            CreateMap<CreateRoleDTO, Role>();
            CreateMap<Role, RoleListDTO>();
            CreateMap<Role, RoleDTO>();
        }
    }
}

