using AutoMapper;
using BackEnd.DTOs.Warehouses.Stocks;
using BackEnd.Entities.Warehouses;

namespace BackEnd.Profiles.Warehouses
{
    public class StocksProfile : Profile
    {
        public StocksProfile()
        {

            CreateMap<CreateStockDTO, Stock>();

            CreateMap<Stock, StockListDTO>()
                .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<Stock, StockDTO>()
                .ForMember(des => des.Product, opt => opt.MapFrom(src => src.Item))
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();
        }
    }
}
