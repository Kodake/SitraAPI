using AutoMapper;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.Entities.Invoices;

namespace SITRA.API.Profiles.Invoices
{
    public class InvoicesProfiles : Profile
    {
        public InvoicesProfiles()
        {
            // Sale Invoices
            CreateMap<CreateSaleInvoiceDTO, Invoice>()
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Payable, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.Warehouse, opt => opt.Ignore());

            CreateMap<CreateQuotationDTO, Invoice>()
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.Quotation, opt => opt.Ignore())
                .ForMember(des => des.Warehouse, opt => opt.Ignore());

            CreateMap<Invoice, SaleInvoiceDTO>()
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.Buyer : null))
                .ForMember(des => des.CustomerId, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.ContactId : 0))
                .ForMember(des => des.Biller, opt => opt.MapFrom(src => src.InvoiceBiller != null ? src.InvoiceBiller.Biller : null))
                .ForMember(des => des.BillerId, opt => opt.MapFrom(src => src.InvoiceBiller != null ? src.InvoiceBiller.ContactId : 0))
                .ForMember(des => des.FiscalSupport, opt => opt.MapFrom(src => src.InvoiceFiscalSupport != null ? src.InvoiceFiscalSupport.FiscalSupport : null));
            CreateMap<SaleInvoiceItemDTO, InvoiceItem>();
            CreateMap<Invoice, SaleInvoiceListDTO>()
                .ForMember(des => des.ContactId, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.ContactId : (long?)null))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.Buyer : null));


            CreateMap<SaleInvoiceItemDTO, InvoiceItem>()
                .ForMember(des => des.Item, opt => opt.Ignore());

            CreateMap<InvoiceItem, SaleInvoiceItemDTO>()
                .ForMember(des => des.ItemType, opt => opt.MapFrom(src => src.Item.ItemType))
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.Item.ItemTaxes));

            // Purchase Invoices
            CreateMap<CreatePurchaseInvoiceDTO, Invoice>()
                .ForMember(des => des.InvoiceBuyer, opt => opt.Ignore())
                .ForMember(des => des.Pricing, opt => opt.Ignore())
                .ForMember(des => des.Payable, opt => opt.Ignore())
                .ForMember(des => des.Receivable, opt => opt.Ignore())
                .ForMember(des => des.Warehouse, opt => opt.Ignore());

            CreateMap<Invoice, PurchaseInvoiceDTO>()
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.Buyer : null))
                .ForMember(des => des.SupplierId, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.ContactId : 0));
            CreateMap<PurchaseInvoiceItemDTO, InvoiceItem>();
            CreateMap<Invoice, PurchaseInvoiceListDTO>()
                .ForMember(des => des.SupplierId, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.ContactId : (long?)null))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.InvoiceBuyer != null ? src.InvoiceBuyer.Buyer : null));

            CreateMap<PurchaseInvoiceItemDTO, InvoiceItem>()
                .ForMember(des => des.Item, opt => opt.Ignore());

            CreateMap<InvoiceItem, PurchaseInvoiceItemDTO>()
                .ForMember(des => des.ItemType, opt => opt.MapFrom(src => src.Item.ItemType))
                .ForMember(des => des.ItemTaxes, opt => opt.MapFrom(src => src.Item.ItemTaxes));
        }
    }
}

