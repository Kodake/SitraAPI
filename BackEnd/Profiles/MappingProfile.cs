using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PaymentDTO, Payment>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        }
    }
}
