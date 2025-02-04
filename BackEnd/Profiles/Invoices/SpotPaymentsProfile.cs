using AutoMapper;
using BackEnd.DTOs.Invoices.SpotPayments;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
{
    public class SpotPaymentsProfile : Profile
    {
        public SpotPaymentsProfile()
        {
            CreateMap<SpotPayment, SpotPaymentListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<SpotPayment, SpotPaymentDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));
        }
    }
}
