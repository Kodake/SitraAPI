using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.API.Profiles.Warehouses
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

