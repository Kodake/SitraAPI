using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.Entities.Finances;

namespace BackEnd.Profiles.Finances
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
