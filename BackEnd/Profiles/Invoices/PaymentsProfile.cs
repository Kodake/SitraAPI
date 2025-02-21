using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
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