using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Contacts.Contacts;
using BackEnd.Entities.Contacts;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Endpoints
{
    public static class ContactsEndpoints
    {
        public static RouteGroupBuilder MapContacts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("contacts-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/customers", GetCustomers);
            group.MapGet("/billers", GetBillers);
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateContactDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateContactDTO>>()
                //.RequireAuthorization()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            return group;
        }

        static async Task<Ok<List<ContactListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var contacts = await unitOfWork.ContactRepository.GetAllAsync(pagination);
            var contactsDTO = mapper.Map<List<ContactListDTO>>(contacts);

            return TypedResults.Ok(contactsDTO);
        }

        static async Task<Ok<List<ContactListDTO>>> GetCustomers(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper)
        {
            var contacts = await unitOfWork.ContactRepository.GetCustomers();
            var contactsDTO = mapper.Map<List<ContactListDTO>>(contacts);

            return TypedResults.Ok(contactsDTO);
        }

        static async Task<Ok<List<ContactListDTO>>> GetBillers(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper)
        {
            var contacts = await unitOfWork.ContactRepository.GetBillers();
            var contactsDTO = mapper.Map<List<ContactListDTO>>(contacts);

            return TypedResults.Ok(contactsDTO);
        }

        static async Task<Results<Ok<ContactDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var contact = await unitOfWork.ContactRepository.GetByIdAsync(id);

            if (contact is null)
            {
                return TypedResults.NotFound();
            }

            var contactDTO = mapper.Map<ContactDTO>(contact);

            return TypedResults.Ok(contactDTO);
        }

        static async Task<Results<Created<ContactDTO>, ValidationProblem>> Create([FromBody] CreateContactDTO dto,
           [FromServices] IUnitOfWork unitOfWork,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var contact = mapper.Map<Contact>(dto);

            await unitOfWork.ContactRepository.CreateAsync(contact);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", default);

            var contactDTO = mapper.Map<ContactDTO>(contact);

            return TypedResults.Created($"/contacts/{contact.Id}", contactDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateContactDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(id);

            if (contactDB is null)
            {
                return TypedResults.NotFound();
            }

            contactDB.ContactType = dto.ContactType;
            contactDB.IdentificationType = dto.IdentificationType;
            contactDB.Province = dto.Province;
            contactDB.Identification = dto.Identification;
            contactDB.Name = dto.Name;
            contactDB.Address = dto.Address;
            contactDB.Phone = dto.Phone;
            contactDB.Phone2 = dto.Phone2;
            contactDB.Email = dto.Email;
            contactDB.IsActive = contactDB.IsActive;

            unitOfWork.ContactRepository.Update(contactDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(id);

            if (contactDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ContactRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var contactDB = await unitOfWork.ContactRepository.GetByIdAsync(id);

            if (contactDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ContactRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("contacts-get", default);

            return TypedResults.Ok();
        }
    }
}
