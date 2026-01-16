using AutoMapper;
using SITRA.Core.DTOs.Products.Units;
using SITRA.Core.Entities.Products;

namespace SITRA.API.Profiles.Products
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

