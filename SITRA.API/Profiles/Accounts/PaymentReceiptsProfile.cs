using AutoMapper;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.Entities.Accounts;

namespace SITRA.API.Profiles.Accounts
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

