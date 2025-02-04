using AutoMapper;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Entities.Accounts;

namespace BackEnd.Profiles.Accounts
{
    public class ReceivablesProfile : Profile
    {
        public ReceivablesProfile()
        {
            CreateMap<Receivable, ReceivableListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<Receivable, ReceivableDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer))
                .ForMember(des => des.PaymentReceipts, opt => opt.MapFrom(src => src.PaymentReceipts));
        }
    }
}
