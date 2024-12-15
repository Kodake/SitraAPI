using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products;
using BackEnd.Entities.Products;
using BackEnd.Enums;
using BackEnd.Filters;
using BackEnd.Helpers;
using BackEnd.Repositories;
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

        static async Task<Ok<List<ServiceListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var services = await unitOfWork.ServiceRepository.GetAllAsync(pagination);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);

            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Ok<List<ServiceListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var services = await unitOfWork.ServiceRepository.GetAll(status);
            var servicesDTO = mapper.Map<List<ServiceListDTO>>(services);

            return TypedResults.Ok(servicesDTO);
        }

        static async Task<Results<Ok<ServiceDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);

            if (service is null)
            {
                return TypedResults.NotFound();
            }

            var serviceDTO = mapper.Map<ServiceDTO>(service);

            return TypedResults.Ok(serviceDTO);
        }

        static async Task<Results<Created<ServiceDTO>, ValidationProblem>> Create(
            [FromBody] CreateServiceDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            try
            {
                var service = mapper.Map<Item>(dto);
                service.ItemType = ItemType.Service; // Crear un Producto

                var serviceSaved = await unitOfWork.ServiceRepository.CreateAsync(service);

                var pricing = await unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
                if (pricing == null)
                {
                    pricing = new Pricing
                    {
                        Name = Enum.GetName(PricingName.Principal) ?? "Principal",
                        PricingType = PricingType.Value
                    };
                    pricing = await unitOfWork.PricingRepository.CreateAsync(pricing);
                }

                var pricingItem = new PricingItem
                {
                    Item = service,
                    Pricing = pricing,
                    InitialCost = dto.InitialCost,
                    BasePrice = dto.BasePrice,
                    IsPrincipal = true
                };

                await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("pricings-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);

                var serviceDTO = mapper.Map<ServiceDTO>(service);
                return TypedResults.Created($"/services/{serviceSaved.Id}", serviceDTO);
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"An error occurred while creating the service: {ex.Message}" } }
                });
            }
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateServiceDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            try
            {
                var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(id);
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
                    var pricingItem = await unitOfWork.PricingItemRepository
                        .GetPricingItemByItemIdAndPricingId(id, principalPricingItem.PricingId);

                    if (pricingItem != null)
                    {
                        pricingItem.InitialCost = dto.InitialCost;
                        pricingItem.BasePrice = dto.BasePrice;
                        unitOfWork.PricingItemRepository.Update(pricingItem);
                    }
                }

                unitOfWork.ServiceRepository.Update(serviceDB);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);

                return TypedResults.Ok();
            }
            catch (Exception)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while updating the product." } }
                });
            }
        }

        public static async Task<Results<Ok, ValidationProblem>> UploadCsv(
            [FromForm] CsvDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper,
            IOutputCacheStore outputCacheStore,
            [FromServices] IValidator<CreateServiceCsvDTO> validator,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                var validFileType = configuration.GetValue<string>("CsvUpload:ValidFileType");
                var validFileName = configuration.GetValue<string>("CsvUpload:ServiceFileName");

                if (dto.CsvFile.ContentType != validFileType)
                {
                    return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "Error", new[] { "Invalid file format. Only CSV files are allowed." } }
                    });
                }

                if (dto.CsvFile.FileName != validFileName)
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
                    var serviceSaved = await unitOfWork.ServiceRepository.CreateAsync(service);

                    var pricing = await unitOfWork.PricingRepository.GetPricingByName(Enum.GetName(PricingName.Principal));
                    if (pricing == null)
                    {
                        pricing = new Pricing
                        {
                            Name = Enum.GetName(PricingName.Principal) ?? "Principal",
                            PricingType = PricingType.Value
                        };
                        pricing = await unitOfWork.PricingRepository.CreateAsync(pricing);
                    }

                    var pricingItem = new PricingItem
                    {
                        Item = serviceSaved,
                        Pricing = pricing,
                        BasePrice = record.BasePrice,
                        IsPrincipal = true
                    };

                    await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
                }

                if (errors.Count > 0)
                {
                    return TypedResults.ValidationProblem(errors);
                }

                await unitOfWork.SaveChangesAsync();

                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("pricings-get", default);
                await outputCacheStore.EvictByTagAsync("services-get", default);

                return TypedResults.Ok();
            }
            catch (Exception)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "An error occurred while processing the CSV." } }
                });
            }
        }

        static async Task<Results<NotFound, Ok>> Activate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ServiceRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("services-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var serviceDB = await unitOfWork.ServiceRepository.GetByIdAsync(id);

            if (serviceDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ServiceRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("services-get", default);

            return TypedResults.Ok();
        }
    }
}
