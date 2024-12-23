using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Entities.AdministrativeTools;

namespace BackEnd.Profiles.AdministrativeTools
{
    public class FiscalSupportsProfile : Profile
    {
        public FiscalSupportsProfile()
        {
            CreateMap<FiscalSupport, FiscalSupportDTO>()
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateFiscalSupportDTO, FiscalSupport>();

            CreateMap<FiscalSupport, FiscalSupportListDTO>()
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();
        }
    }
}
