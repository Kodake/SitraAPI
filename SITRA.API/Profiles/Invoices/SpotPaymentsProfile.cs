using AutoMapper;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.Entities.Invoices;

namespace SITRA.API.Profiles.Invoices
{
    public class SpotPaymentsProfile : Profile
    {
        public SpotPaymentsProfile()
        {
            CreateMap<SpotPayment, SpotPaymentListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer))
                .ForMember(des => des.AmountReceived, opt => opt.MapFrom(src =>
                   src.Payments.Sum(x => x.Amount)));

            CreateMap<SpotPayment, SpotPaymentDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));
        }
    }
}

