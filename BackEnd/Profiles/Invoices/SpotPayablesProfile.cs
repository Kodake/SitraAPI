using AutoMapper;
using BackEnd.DTOs.Invoices.SpotPayables;
using BackEnd.Entities.Invoices;

namespace BackEnd.Profiles.Invoices
{
    public class SpotPayablesProfile : Profile
    {
        public SpotPayablesProfile()
        {
            CreateMap<SpotPayable, SpotPayableListDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier));

            CreateMap<SpotPayable, SpotPayableDTO>()
                .ForMember(des => des.Invoice, opt => opt.MapFrom(src => src.Invoice))
                .ForMember(des => des.Supplier, opt => opt.MapFrom(src => src.Supplier));
        }
    }
}
