using AutoMapper;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Entities.Contacts;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Contacts
{
    public class UpdateContactCommand : UpdateCommand<Contact, CreateContactDTO>
    {
        public UpdateContactCommand(long id, CreateContactDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(Id);

            if (contactDB == null) return false;

            mapper.Map(Dto, contactDB);
            unitOfWork.ContactRepository.Update(contactDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", cancellationToken);

            return true;
        }
    }
}
