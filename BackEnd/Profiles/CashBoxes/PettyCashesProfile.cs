using AutoMapper;
using BackEnd.DTOs.Auths;
using BackEnd.DTOs.CashBoxes;
using BackEnd.Entities.Auths;
using BackEnd.Entities.CashBoxes;

namespace BackEnd.Profiles.CashBoxes
{
    public class PettyCashesProfile : Profile
    {
        public PettyCashesProfile()
        {
            CreateMap<CreateOpenPettyCashDTO, PettyCash>();
            CreateMap<CreateClosePettyCashDTO, PettyCash>();

            CreateMap<PettyCash, PettyCashDTO>()
                .ForMember(des => des.User, opt => opt.MapFrom(src => src.User))
                .ForMember(des => des.PettyCashMovements, opt => opt.MapFrom(src => src.PettyCashMovements));

            CreateMap<User, UserDTO>();

            CreateMap<PettyCashMovement, PettyCashMovementListDTO>();
        }
    }
}
