using AutoMapper;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Profiles.Warehouses
{
    public class TransfersProfile : Profile
    {
        public TransfersProfile()
        {
            CreateMap<CreateTransferDTO, Transfer>();

            CreateMap<CreateTransferStatusDTO, Transfer>();

            CreateMap<Transfer, TransferDTO>()
            .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(des => des.OriginWarehouse, opt => opt.MapFrom(src => src.OriginWarehouse))
            .ForMember(des => des.DestinationWarehouse, opt => opt.MapFrom(src => src.DestinationWarehouse))
            .ForMember(des => des.Stock, opt => opt.MapFrom(src => src.Stock));

            CreateMap<Transfer, TransferListDTO>()
            .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(des => des.OriginWarehouse, opt => opt.MapFrom(src => src.OriginWarehouse))
            .ForMember(des => des.DestinationWarehouse, opt => opt.MapFrom(src => src.DestinationWarehouse))
            .ForMember(des => des.Stock, opt => opt.MapFrom(src => src.Stock));
        }
    }
}
