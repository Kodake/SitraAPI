using AutoMapper;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;

namespace SITRA.API.Profiles
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

