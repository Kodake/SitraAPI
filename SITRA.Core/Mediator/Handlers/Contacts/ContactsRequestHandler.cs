using Microsoft.Extensions.DependencyInjection;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Mediator.Commands.Contacts;
using SITRA.Core.Mediator.Queries.Contacts;
using MediatR;

namespace SITRA.Core.Mediator.Handlers.Contacts
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

