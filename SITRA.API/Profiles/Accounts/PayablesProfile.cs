using AutoMapper;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.Entities.Accounts;

namespace SITRA.API.Profiles.Accounts
{
    public class PayablesProfile : Profile
    {
        public PayablesProfile()
        {
            CreateMap<Payable, PayableListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier));

            CreateMap<Payable, PayableDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier))
                .ForMember(des => des.BillerId, opt => opt.MapFrom(src => src.Invoice.InvoiceBiller != null ? src.Invoice.InvoiceBiller.ContactId : (Guid?)null))
                .ForMember(des => des.Biller, opt => opt.MapFrom(src => src.Invoice.InvoiceBiller != null ? src.Invoice.InvoiceBiller.Biller : null))
                .ForMember(des => des.PayableReceipts, opt => opt.MapFrom(src => src.PayableReceipts));
        }
    }
}

