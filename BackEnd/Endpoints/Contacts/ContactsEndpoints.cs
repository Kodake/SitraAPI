using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Contacts;
using BackEnd.Entities.Contacts;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Contacts;
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

        static async Task<Ok<List<ContactListDTO>>> GetAll([FromServices] IContactRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var contacts = await repository.GetAll(pagination);
            var contactsDTO = mapper.Map<List<ContactListDTO>>(contacts);
            return TypedResults.Ok(contactsDTO);
        }

        static async Task<Results<Ok<ContactDTO>, NotFound>> GetById([FromServices] IContactRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var contact = await repository.GetById(id);

            if (contact is null)
            {
                return TypedResults.NotFound();
            }

            var contactDTO = mapper.Map<ContactDTO>(contact);
            return TypedResults.Ok(contactDTO);
        }

        static async Task<Results<Created<ContactDTO>, ValidationProblem>> Create([FromBody] CreateContactDTO dto,
           [FromServices] IContactRepository repository,
           IOutputCacheStore outputCacheStore,
           [FromServices] IMapper mapper)
        {
            var contact = mapper.Map<Contact>(dto);

            var id = await repository.Create(contact);
            await outputCacheStore.EvictByTagAsync("contacts-get", default);
            var contactDTO = mapper.Map<ContactDTO>(contact);
            return TypedResults.Created($"/contacts/{id}", contactDTO);
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateContactDTO dto,
            [FromServices] IContactRepository repository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var contactDB = await repository.GetById(id);

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
            contactDB.IsActive = contactDB.IsActive;

            await repository.Update(contactDB);
            await outputCacheStore.EvictByTagAsync("contacts-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IContactRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var contactDB = await repository.GetById(id);

            if (contactDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("contacts-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IContactRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var contactDB = await repository.GetById(id);

            if (contactDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("contacts-get", default);
            return TypedResults.Ok();
        }
    }
}
