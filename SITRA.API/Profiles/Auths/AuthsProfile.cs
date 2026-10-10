using AutoMapper;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Entities.Auths;

namespace SITRA.API.Profiles.Auths
{
    public class AuthsProfile : Profile
    {
        public AuthsProfile()
        {
            CreateMap<CreateRegistrationUserDTO, User>()
                .ForMember(des => des.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}

