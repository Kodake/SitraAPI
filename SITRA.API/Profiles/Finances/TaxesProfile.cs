using AutoMapper;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.Entities.Finances;

namespace SITRA.API.Profiles.Finances
{
    public class TaxesProfile : Profile
    {
        public TaxesProfile()
        {
            CreateMap<CreateTaxDTO, Tax>();
            CreateMap<Tax, TaxListDTO>();
            CreateMap<Tax, TaxDTO>();
        }
    }
}

