using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Entities.Contacts;

namespace BackEnd.Profiles.Contacts
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
