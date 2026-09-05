using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Mediator.Commands.Contacts
{
    public class UpdateContactCommand : UpdateCommand<Contact, CreateContactDTO>
    {
        public UpdateContactCommand(Guid id, CreateContactDTO dto) : base(id, dto) { }

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
