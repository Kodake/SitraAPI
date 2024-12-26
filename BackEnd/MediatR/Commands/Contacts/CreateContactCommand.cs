using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Entities.Contacts;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.MediatR.Commands.Contacts
{
    public class CreateContactCommand : CreateCommand<Contact, CreateContactDTO>
    {
        public CreateContactCommand(CreateContactDTO dto) : base(dto) { }

        public override async Task<CreateContactDTO> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var contact = mapper.Map<Contact>(Dto);
            await unitOfWork.ContactRepository.CreateAsync(contact);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", cancellationToken);
            return Dto;
        }
    }
}
