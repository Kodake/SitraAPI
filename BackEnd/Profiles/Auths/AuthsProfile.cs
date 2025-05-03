using AutoMapper;
using BackEnd.DTOs.Auths;
using BackEnd.Entities.Auths;

namespace BackEnd.Profiles.Auths
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
