using AutoMapper;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Entities.Contacts;

namespace SITRA.API.Profiles.Contacts
{
    public class ContactsProfile : Profile
    {
        public ContactsProfile()
        {
            CreateMap<CreateContactDTO, Contact>();
            CreateMap<Contact, ContactListDTO>();
            CreateMap<Contact, ContactDTO>();
        }
    }
}

