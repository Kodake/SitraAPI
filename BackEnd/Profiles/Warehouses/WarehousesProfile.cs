using AutoMapper;
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
        }
    }
}
