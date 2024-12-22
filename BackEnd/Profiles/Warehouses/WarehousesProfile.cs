using AutoMapper;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.DTOs.Warehouses.Transfers;
using BackEnd.DTOs.Warehouses.Warehouses;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Profiles.Warehouses
{
    public class WarehousesProfile : Profile
    {
        public WarehousesProfile()
        {
            CreateMap<CreateWarehouseDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseListDTO>();
            CreateMap<Warehouse, WarehouseDTO>();


            CreateMap<Stock, StockDTO>()
                .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateStockDTO, Stock>();

            CreateMap<Stock, StockListDTO>()
                .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

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
