using AutoMapper;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Entities.Appointments;

namespace SITRA.API.Profiles.Appointments
{
    public class AppointmentsProfile: Profile
    {
        public AppointmentsProfile()
        {
            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Contact, opt => opt.Ignore());
            CreateMap<Appointment, AppointmentListDTO>()
                .ForMember(dest => dest.ContactName,
                    opt => opt.MapFrom(src => src.Contact.Name))
                .ForMember(dest => dest.ContactEmail,
                    opt => opt.MapFrom(src => src.Contact.Email))
                .ForMember(dest => dest.ContactPhone,
                    opt => opt.MapFrom(src => src.Contact.Phone));
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(des => des.AppointmentItems, opt => opt.MapFrom(src => src.AppointmentItems.Select(pt => new ServiceDTO
                {
                    Id = pt.Item.Id,
                    Name = pt.Item.Name,
                    Description = pt.Item.Description,
                }).ToList()))
                .ForMember(des => des.Contact, opt => opt.MapFrom(src => src.Contact != null ? new ContactDTO
                {
                    Id = src.Id,
                    Name = src.Contact.Name,
                    Identification = src.Contact.Identification,
                    Email = src.Contact.Email,
                    Phone = src.Contact.Phone,
                    Phone2 = src.Contact.Phone2
                } : null))
                .ForMember(des => des.AppointmentItemIds, opt => opt.MapFrom(src => src.AppointmentItems.Select(pt => pt.Item.Id).ToList()));
        }
    }
}

