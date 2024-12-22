using AutoMapper;
using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.DTOs.Accounts.Receivables;
using BackEnd.Entities.Accounts;

namespace BackEnd.Profiles.Accounts
{
    public class AccountsProfile : Profile
    {
        public AccountsProfile()
        {
            CreateMap<Receivable, ReceivableListDTO>()
                .ForMember(des => des.SaleInvoice, opt => opt.MapFrom(src => src.SaleInvoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer));

            CreateMap<Receivable, ReceivableDTO>()
                .ForMember(des => des.SaleInvoice, opt => opt.MapFrom(src => src.SaleInvoice))
                .ForMember(des => des.Buyer, opt => opt.MapFrom(src => src.Buyer))
                .ForMember(des => des.PaymentReceipts, opt => opt.MapFrom(src => src.PaymentReceipts));

            CreateMap<CreatePaymentReceiptDTO, PaymentReceipt>()
                .ForMember(des => des.Receivable, opt => opt.Ignore());

            CreateMap<PaymentReceipt, PaymentReceiptDTO>()
                .ForMember(des => des.Receivable, opt => opt.MapFrom(src => src.Receivable));

            CreateMap<PaymentReceipt, PaymentReceiptListDTO>();
        }
    }
}
