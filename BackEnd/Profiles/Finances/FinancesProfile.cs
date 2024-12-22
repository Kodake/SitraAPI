using AutoMapper;
using BackEnd.DTOs.Finances.Currencies;
using BackEnd.DTOs.Finances.Taxes;
using BackEnd.Entities.Finances;

namespace BackEnd.Profiles.Finances
{
    public class FinancesProfile : Profile
    {
        public FinancesProfile()
        {
            CreateMap<CreateCurrencyDTO, Currency>();
            CreateMap<Currency, CurrencyListDTO>();
            CreateMap<Currency, CurrencyDTO>();

            CreateMap<CreateTaxDTO, Tax>();
            CreateMap<Tax, TaxListDTO>();
            CreateMap<Tax, TaxDTO>();
        }
    }
}
