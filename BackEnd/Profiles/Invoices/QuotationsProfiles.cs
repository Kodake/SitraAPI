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
                .ForMember(des => des.SaleInvoice, opt => opt.MapFrom(src => src.SaleInvoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<Quotation, QuotationDTO>()
                .ForMember(des => des.SaleInvoice, opt => opt.MapFrom(src => src.SaleInvoice))
                .ForMember(des => des.SaleInvoiceItems, opt => opt.MapFrom(src => src.SaleInvoice.SaleInvoiceItems))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));
        }
    }
}
