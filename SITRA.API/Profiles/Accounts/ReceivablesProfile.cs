using AutoMapper;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.Entities.Accounts;

namespace SITRA.API.Profiles.Accounts
{
    public class ReceivablesProfile : Profile
    {
        public ReceivablesProfile()
        {
            CreateMap<Receivable, ReceivableListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<Receivable, ReceivableDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer))
                .ForMember(des => des.BillerId, opt => opt.MapFrom(src => src.Invoice.InvoiceBiller != null ? src.Invoice.InvoiceBiller.ContactId : (Guid?)null))
                .ForMember(des => des.Biller, opt => opt.MapFrom(src => src.Invoice.InvoiceBiller != null ? src.Invoice.InvoiceBiller.Biller : null))
                .ForMember(des => des.PaymentReceipts, opt => opt.MapFrom(src => src.PaymentReceipts));
        }
    }
}

