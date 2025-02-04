using AutoMapper;
using BackEnd.DTOs.Invoices.SaleInvoices;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
{
    public class InvoicesProfiles : Profile
    {
        public InvoicesProfiles()
        {
            CreateMap<CreateSaleInvoiceDTO, Invoice>()
                .ForMember(des => des.Buyer, opt => opt.Ignore())
                .ForMember(des => des.Biller, opt => opt.Ignore())
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.FiscalSupport, opt => opt.Ignore())
                .ForMember(des => des.Warehouse, opt => opt.Ignore());

            CreateMap<CreateQuotationDTO, Invoice>()
                .ForMember(des => des.Buyer, opt => opt.Ignore())
                .ForMember(des => des.Biller, opt => opt.Ignore())
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.Quotation, opt => opt.Ignore())
                .ForMember(des => des.FiscalSupport, opt => opt.Ignore())
                .ForMember(des => des.Warehouse, opt => opt.Ignore());

            CreateMap<Invoice, SaleInvoiceDTO>();
            CreateMap<SaleInvoiceItemDTO, InvoiceItem>();
            CreateMap<Invoice, SaleInvoiceListDTO>();

            CreateMap<SaleInvoiceItemDTO, InvoiceItem>()
                .ForMember(des => des.Item, opt => opt.Ignore());

            CreateMap<InvoiceItem, SaleInvoiceItemDTO>()
                .ForMember(des => des.ItemType, opt => opt.MapFrom(src => src.Item.ItemType))
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.Item.ItemTaxes));
        }
    }
}
