using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Mediator.Commands.Contacts;
using BackEnd.Mediator.Queries.Contacts;
using MediatR;

namespace BackEnd.Mediator.Handlers.Contacts
{
    public static class ContactsRequestHandler
    {
        public static IServiceCollection AddContactServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<ActivateContactCommand, bool>, ToggleCommandHandler<ActivateContactCommand>>();
            services.AddTransient<IRequestHandler<DeactivateContactCommand, bool>, ToggleCommandHandler<DeactivateContactCommand>>();
            services.AddTransient<IRequestHandler<GetContactByIdQuery, ContactDTO>, QueryHandler<GetContactByIdQuery, ContactDTO>>();
            services.AddTransient<IRequestHandler<GetSelectsBillersQuery, List<ContactListDTO>>, QueryHandler<GetSelectsBillersQuery, List<ContactListDTO>>>();
            services.AddTransient<IRequestHandler<GetSelectsCustomersQuery, List<ContactListDTO>>, QueryHandler<GetSelectsCustomersQuery, List<ContactListDTO>>>();
            services.AddTransient<IRequestHandler<GetSelectsSuppliersQuery, List<ContactListDTO>>, QueryHandler<GetSelectsSuppliersQuery, List<ContactListDTO>>>();
            services.AddTransient<IRequestHandler<GetAllContactsQuery, List<ContactListDTO>>, QueryHandler<GetAllContactsQuery, List<ContactListDTO>>>();
            services.AddTransient<IRequestHandler<CreateContactCommand, CreateContactDTO>, CreateCommandHandler<Entities.Contacts.Contact, CreateContactDTO>>();
            services.AddTransient<IRequestHandler<UpdateContactCommand, bool>, UpdateCommandHandler<Entities.Contacts.Contact, CreateContactDTO>>();

            return services;
        }
    }
}
