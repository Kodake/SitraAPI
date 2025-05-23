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
                .ForMember(des => des.PaymentMethods, opt => opt.Ignore())
                .ForMember(des => des.User, opt => opt.MapFrom(src => src.User));

            CreateMap<User, UserDTO>();

            CreateMap<PettyCashMovement, PettyCashMovementListDTO>()
                .ForMember(des => des.FiscalSupportNumber, opt => opt.MapFrom(src => src.Invoice!.FiscalSupportNumber));

            CreateMap<CreateWithdrawalPettyCashMovementDTO, PettyCashMovement>();
            CreateMap<PettyCashMovement, WithdrawalPettyCashMovementListDTO>();
        }
    }
}
