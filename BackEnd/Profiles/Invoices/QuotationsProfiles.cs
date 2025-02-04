using AutoMapper;
using BackEnd.DTOs.Invoices.Quotations;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
{
    public class QuotationsProfiles : Profile
    {
        public QuotationsProfiles()
        {
            CreateMap<Quotation, QuotationListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<Quotation, QuotationDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.InvoiceItems, opt => opt.MapFrom(src => src.Invoice.InvoiceItems))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));
        }
    }
}
