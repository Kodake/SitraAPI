using AutoMapper;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.Entities.Warehouses;

namespace SITRA.API.Profiles.Warehouses
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

