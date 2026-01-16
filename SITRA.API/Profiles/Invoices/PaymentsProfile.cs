using AutoMapper;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;

namespace SITRA.API.Profiles.Invoices
{
    public class PaymentsProfile : Profile
    {
        public PaymentsProfile()
        {
            CreateMap<Payment, PaymentDTO>()
                .ForMember(des => des.PaymentId, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(des => des.Amount, opt => opt.MapFrom(src => src.Amount));
        }
    }
}

