using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
{
    public class InvoicesProfiles : Profile
    {
        public InvoicesProfiles()
        {
            CreateMap<CreateSaleInvoiceDTO, SaleInvoice>()
                .ForMember(des => des.Buyer, opt => opt.Ignore())
                .ForMember(des => des.Biller, opt => opt.Ignore())
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.FiscalSupport, opt => opt.Ignore());

            CreateMap<SaleInvoice, SaleInvoiceDTO>();

            CreateMap<SaleInvoiceItemDTO, SaleInvoiceItem>();

            CreateMap<SaleInvoice, SaleInvoiceListDTO>();
        }
    }
}
