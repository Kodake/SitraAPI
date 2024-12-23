using AutoMapper;
using BackEnd.DTOs.Products.Units;
using BackEnd.Entities.Products;

namespace BackEnd.Profiles.Products
{
    public class UnitsProfile : Profile
    {
        public UnitsProfile()
        {
            CreateMap<CreateUnitDTO, Unit>();
            CreateMap<Unit, UnitListDTO>();
            CreateMap<Unit, UnitDTO>();
        }
    }
}
