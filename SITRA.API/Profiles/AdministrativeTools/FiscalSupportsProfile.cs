using AutoMapper;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Entities.AdministrativeTools;

namespace SITRA.API.Profiles.AdministrativeTools
{
    public class FiscalSupportsProfile : Profile
    {
        public FiscalSupportsProfile()
        {
            CreateMap<FiscalSupport, FiscalSupportDTO>()
                .ReverseMap();

            CreateMap<CreateFiscalSupportDTO, FiscalSupport>();

            CreateMap<FiscalSupport, FiscalSupportListDTO>()
                .ReverseMap();
        }
    }
}

