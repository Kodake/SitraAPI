using AutoMapper;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.Entities.Accounts;

namespace SITRA.API.Profiles.Accounts
{
    public class PayableReceiptsProfile : Profile
    {
        public PayableReceiptsProfile()
        {
            CreateMap<CreatePayableReceiptDTO, PayableReceipt>()
                .ForMember(des => des.Payable, opt => opt.Ignore());

            CreateMap<PayableReceipt, PayableReceiptDTO>()
                .ForMember(des => des.Payable, opt => opt.MapFrom(src => src.Payable));

            CreateMap<PayableReceipt, PayableReceiptListDTO>();
        }
    }
}

