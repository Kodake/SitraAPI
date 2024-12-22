using AutoMapper;
using BackEnd.DTOs.AdministrativeTools.Deadlines;
using BackEnd.DTOs.AdministrativeTools.FiscalSupports;
using BackEnd.Entities.AdministrativeTools;

namespace BackEnd.Profiles.AdministrativeTools
{
    public class AdministrativeToolsProfile : Profile
    {
        public AdministrativeToolsProfile()
        {
            CreateMap<FiscalSupport, FiscalSupportDTO>()
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateFiscalSupportDTO, FiscalSupport>();

            CreateMap<FiscalSupport, FiscalSupportListDTO>()
                .ForMember(des => des.Warehouse, opt => opt.MapFrom(src => src.Warehouse))
                .ReverseMap();

            CreateMap<CreateDeadlineDTO, Deadline>();
            CreateMap<Deadline, DeadlineListDTO>();
            CreateMap<Deadline, DeadlineDTO>();
        }
    }
}
