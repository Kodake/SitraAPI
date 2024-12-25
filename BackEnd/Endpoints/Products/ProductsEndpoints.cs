using AutoMapper;
using BackEnd.DTOs;
using BackEnd.DTOs.Products.Products;
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

namespace BackEnd.Endpoints
{
    public static class ProductsEndpoints
    {
        public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"))
                .AddPaginationParamsOpenAPI();
            group.MapGet("/stock-products", GetStockProducts);
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateProductDTO>>()
                .WithOpenApi();
            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateProductDTO>>()
                .WithOpenApi();
            group.MapPatch("/{id:int}/activate", Activate);
            group.MapDelete("/{id:int}/deactivate", Deactivate);
            //.RequireAuthorization("isAdmin");
            group.MapPost("/upload-csv", UploadCsv).DisableAntiforgery()
                .WithOpenApi();
            return group;
        }

        static async Task<Ok<List<ProductListDTO>>> GetAll(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, PaginationDTO pagination)
        {
            var products = await unitOfWork.ProductRepository.GetAllAsync(pagination);
            var productsDTO = mapper.Map<List<ProductListDTO>>(products);

            return TypedResults.Ok(productsDTO);
        }

        static async Task<Ok<List<ProductListDTO>>> GetSelects(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper, [FromQuery] StatusType status = StatusType.All)
        {
            var products = await unitOfWork.ProductRepository.GetAll(status);
            var productsDTO = mapper.Map<List<ProductListDTO>>(products);

            return TypedResults.Ok(productsDTO);
        }

        static async Task<Ok<List<ProductListDTO>>> GetStockProducts(
            [FromServices] IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            var products = await unitOfWork.ProductRepository.GetAll();
            var productsDTO = mapper.Map<List<ProductListDTO>>(products);

            return TypedResults.Ok(productsDTO);
        }

        static async Task<Results<Ok<ProductDTO>, NotFound>> GetById(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] IMapper mapper, int id)
        {
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (product is null)
            {
                return TypedResults.NotFound();
            }

            var productDTO = mapper.Map<ProductDTO>(product);

            return TypedResults.Ok(productDTO);
        }

        static async Task<Results<Created, ValidationProblem>> Create(
            [FromBody] CreateProductDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            try
            {
                var product = mapper.Map<Item>(dto);
                product.ItemType = ItemType.Product; // Crear un Producto

                var productSaved = await unitOfWork.ProductRepository.CreateAsync(product);

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
                    Item = product,
                    Pricing = pricing,
                    InitialCost = dto.InitialCost,
                    BasePrice = dto.BasePrice,
                    IsPrincipal = true
                };

                await unitOfWork.PricingItemRepository.CreateAsync(pricingItem);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("pricings-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);

                return TypedResults.Created();
            }
            catch (Exception ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { $"An error occurred while creating the product: {ex.Message}" } }
                });
            }
        }

        static async Task<Results<Ok, NotFound, ValidationProblem>> Update(int id, [FromBody] CreateProductDTO dto,
            [FromServices] IUnitOfWork unitOfWork,
            IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            try
            {
                var productDB = await unitOfWork.ProductRepository.GetByIdAsync(id);
                if (productDB is null)
                {
                    return TypedResults.NotFound();
                }

                productDB.Name = dto.Name;
                productDB.Reference = dto.Reference;
                productDB.Description = dto.Description;
                productDB.BrandId = dto.BrandId;
                productDB.CategoryId = dto.CategoryId;
                productDB.UnitId = dto.UnitId;

                productDB.ItemTaxes = dto.TaxIds.Select(taxId => new ItemTaxes
                {
                    TaxId = taxId,
                    ItemId = productDB.Id
                }).ToList();

                var principalPricingItem = productDB.PricingItems.FirstOrDefault(x => x.IsPrincipal);
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

                unitOfWork.ProductRepository.Update(productDB);
                await unitOfWork.SaveChangesAsync();
                await outputCacheStore.EvictByTagAsync("pricing-items-get", default);
                await outputCacheStore.EvictByTagAsync("products-get", default);

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
            [FromServices] IValidator<CreateProductCsvDTO> validator,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                var validFileType = configuration.GetValue<string>("CsvUpload:ValidFileType");
                var validFileName = configuration.GetValue<string>("CsvUpload:ProductFileName");

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

                var records = csv.GetRecords<CreateProductCsvDTO>().ToList();

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
                        errors.Add(record.Name ?? "Unknown Product", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                        continue;
                    }

                    var product = mapper.Map<Item>(record);
                    product.ItemType = ItemType.Product;
                    var productSaved = await unitOfWork.ProductRepository.CreateAsync(product);

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
                        Item = productSaved,
                        Pricing = pricing,
                        InitialCost = record.InitialCost,
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
                await outputCacheStore.EvictByTagAsync("products-get", default);

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
            var productDB = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (productDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ProductRepository.ActivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return TypedResults.Ok();
        }

        static async Task<Results<NotFound, Ok>> Deactivate([FromRoute] int id,
             [FromServices] IUnitOfWork unitOfWork,
             IOutputCacheStore outputCacheStore)
        {
            var productDB = await unitOfWork.ProductRepository.GetByIdAsync(id);

            if (productDB is null)
            {
                return TypedResults.NotFound();
            }

            await unitOfWork.ProductRepository.DeactivateAsync(id);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("products-get", default);

            return TypedResults.Ok();
        }
    }
}