using AutoMapper;
using BackEnd.DTOs.Accounts.PaymentReceipts;
using BackEnd.Entities.Accounts;

namespace BackEnd.Profiles.Accounts
{
    public class PaymentReceiptsProfile : Profile
    {
        public PaymentReceiptsProfile()
        {
            CreateMap<CreatePaymentReceiptDTO, PaymentReceipt>()
                .ForMember(des => des.Receivable, opt => opt.Ignore());

            CreateMap<PaymentReceipt, PaymentReceiptDTO>()
                .ForMember(des => des.Receivable, opt => opt.MapFrom(src => src.Receivable));

            CreateMap<PaymentReceipt, PaymentReceiptListDTO>();
        }
    }
}
