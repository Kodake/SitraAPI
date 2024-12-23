using AutoMapper;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Entities.Finances;

namespace BackEnd.Profiles.Finances
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
