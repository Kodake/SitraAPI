using AutoMapper;
using BackEnd.DTOs.Accounts.Payables;
using BackEnd.Entities.Accounts;

namespace BackEnd.Profiles.Accounts
{
    public class PayablesProfile : Profile
    {
        public PayablesProfile()
        {
            CreateMap<Payable, PayableListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier));

            CreateMap<Payable, PayableDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier))
                .ForMember(des => des.PayableReceipts, opt => opt.MapFrom(src => src.PayableReceipts));
        }
    }
}
