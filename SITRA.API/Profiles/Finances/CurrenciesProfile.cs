using AutoMapper;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.Entities.Finances;

namespace SITRA.API.Profiles.Finances
{
    public class CurrenciesProfile : Profile
    {
        public CurrenciesProfile()
        {
            CreateMap<CreateCurrencyDTO, Currency>();
            CreateMap<Currency, CurrencyListDTO>();
            CreateMap<Currency, CurrencyDTO>();
        }
    }
}

