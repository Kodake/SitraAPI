using AutoMapper;
using BackEnd.Context;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories.Products;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Globalization;

namespace BackEnd.Endpoints.Products
{
    public static class ServicesEndpoints
    {
        public static RouteGroupBuilder MapServices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateServiceDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateServiceDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            group.MapPost("/upload-csv", UploadCsv).DisableAntiforgery()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<ServiceListDTO>>> GetAll([FromServices] IServiceRepository repository,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var services = await repository.GetAll(pagination);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);
            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Ok<List<ServiceListDTO>>> GetSelects(IServiceRepository repository,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var services = await repository.GetAll(status);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);
            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Results<Ok<ServiceDTO>, NotFound>> GetById([FromServices] IServiceRepository repository,
            [FromServices] IMapper mapper, int id)
        {
            var service = await repository.GetById(id);

            if (service is null)
            {
                return TypedResults.NotFound();
            }

            var serviceDTO = mapper.Map<ServiceDTO>(service);
            return TypedResults.Ok(serviceDTO);
        }

        static async Task<Results<Created<ServiceDTO>, ValidationProblem>> Create([FromBody] CreateServiceDTO dto,
            [FromServices] IServiceRepository repository,
            [FromServices] IPricingItemRepository pricingItemRepository,
            [FromServices] IPricingRepository pricingRepository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var service = mapper.Map<Item>(dto);
                service.ItemType = ItemType.Service; // Create a Service

                var id = await repository.Create(service);
                var pricing = await pricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));

                if (pricing == null)
                {
                    pricing = new Pricing
                    {
                        Name = Enum.GetName(PricingName.Principal) ?? "Principal",
                        PricingType = PricingType.Value
                    };

                    pricing = await pricingRepository.Create(pricing);
                }

                var pricingItem = new PricingItem
                {
                    ItemId = id,
                    PricingId = pricing.Id,
                    InitialCost = dto.InitialCost,
                    BasePrice = dto.BasePrice,
                    IsPrincipal = true
                };

                await pricingItemRepository.Create(pricingItem);

                await transaction.CommitAsync();

                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("pricings-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);
                var serviceDTO = mapper.Map<ServiceDTO>(service);
                return TypedResults.Created($"/services/{id}", serviceDTO);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while creating the service." } }
                });
            }
        }

        public static async Task<Results<Ok, ValidationProblem>> UploadCsv(
            [FromForm] CsvDTO dto,
            [FromServices] IServiceRepository repository,
            [FromServices] IPricingItemRepository pricingItemRepository,
            [FromServices] IPricingRepository pricingRepository,
            [FromServices] IMapper mapper,
            IOutputCacheStore outputCacheStore,
            [FromServices] IValidator<CreateServiceCsvDTO> validator,
            [FromServices] ApplicationDbContext context,
            [FromServices] IConfiguration configuration)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var validFileType = configuration.GetValue<string>("CsvUpload:ValidFileType");
                var validFileName = configuration.GetValue<string>("CsvUpload:ServiceFileName");

                var fileType = dto.CsvFile.ContentType;
                if (fileType != validFileType)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Invalid file format. Only CSV files are allowed." } }
                    });
                }

                var fileName = dto.CsvFile.FileName;
                if (fileName != validFileName)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { $"Invalid file name. The file must be named '{validFileName}'." } }
                    });
                }

                using var reader = new StreamReader(dto.CsvFile.OpenReadStream());
                using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<CreateServiceCsvDTO>().ToList();

                var maxFileRecords = configuration.GetValue<int>("CsvUpload:MaxFileRecords");

                if (records.Count > maxFileRecords)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { $"Too many records. The maximum allowed is {maxFileRecords}." } }
                    });
                }

                var errors = new Dictionary<string, string[]>();

                foreach (var record in records)
                {
                    var validationResult = await validator.ValidateAsync(record);

                    if (!validationResult.IsValid)
                    {
                        errors.Add(record.Name ?? "Unknown Service", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                        continue;
                    }

                    var service = mapper.Map<Item>(record);
                    service.ItemType = ItemType.Service;

                    var id = await repository.Create(service);

                    var pricing = await pricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
                    if (pricing == null)
                    {
                        pricing = new Pricing
                        {
                            Name = Enum.GetName(PricingName.Principal) ?? "PRINCIPAL",
                            PricingType = PricingType.Value
                        };

                        pricing = await pricingRepository.Create(pricing);
                    }

                    var pricingItem = new PricingItem
                    {
                        ItemId = id,
                        PricingId = pricing.Id,
                        BasePrice = record.BasePrice,
                        IsPrincipal = true
                    };

                    await pricingItemRepository.Create(pricingItem);
                }

                if (errors.Count > 0)
                {
                    return TypedResults.ValidationProblem(errors);
                }

                await transaction.CommitAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);                
                await outputCacheStore.EvictByTagAsync("pricings-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);

                return TypedResults.Ok();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while processing the CSV." } }
                });
            }
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateServiceDTO dto,
            [FromServices] IServiceRepository repository,
            [FromServices] IPricingItemRepository pricingItemRepository,
            [FromServices] IPricingRepository pricingRepository,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] ApplicationDbContext context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var serviceDB = await repository.GetById(id);
                if (serviceDB is null)
                {
                    return TypedResults.NotFound();
                }

                serviceDB.Name = dto.Name;
                serviceDB.Reference = dto.Reference;
                serviceDB.Description = dto.Description;
                serviceDB.CategoryId = dto.CategoryId;
                serviceDB.UnitId = dto.UnitId;
                serviceDB.ItemTaxes = dto.TaxIds.Select(taxId => new ItemTaxes
                {
                    TaxId = taxId,
                    ItemId = serviceDB.Id
                }).ToList();

                var principalPricingItem = serviceDB.PricingItems.FirstOrDefault(x => x.IsPrincipal);
                if (principalPricingItem != null)
                {
                    var pricingItem = await pricingItemRepository.GetPricingItemByItemIdAndPricingId(id, principalPricingItem.PricingId);
                    if (pricingItem != null)
                    {
                        pricingItem.InitialCost = dto.InitialCost;
                        pricingItem.BasePrice = dto.BasePrice;

                        await pricingItemRepository.Update(pricingItem);
                    }
                }

                await repository.Update(serviceDB);
                await transaction.CommitAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);
                return TypedResults.Ok();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while updating the service." } }
                });
            }
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IServiceRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await repository.GetById(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Activate(id);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IServiceRepository repository,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await repository.GetById(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await repository.Deactivate(id);
            await outputCacheStore.EvictByTagAsync("services-get", default);
            return TypedResults.Ok();
        }
    }
}
