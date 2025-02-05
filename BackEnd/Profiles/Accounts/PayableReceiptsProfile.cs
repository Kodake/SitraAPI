using AutoMapper;
using BackEnd.DTOs.Accounts.PayableReceipts;
using BackEnd.Entities.Accounts;

namespace BackEnd.Profiles.Accounts
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
