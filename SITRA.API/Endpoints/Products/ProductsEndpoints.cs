using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Products.Products;
using SITRA.Core.Mediator.Queries.Products.Products;

namespace SITRA.API.Endpoints
{
    public static class ProductsEndpoints
    {
        public static RouteGroupBuilder MapProducts(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/stock-products", GetStockProducts).RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("products-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPost("/{id:guid}/picture", UploadPicture).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
            //.RequireAuthorization("isAdmin");
            group.MapPost("/upload-csv", UploadCsv).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ProductListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllProductsQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ProductListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsProductsQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ProductListDTO>>> GetStockProducts(
            [FromServices] IMediator mediator)
        {
            var query = new GetStockProductsQuery();
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ProductDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetProductByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<IResult> Create(
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateProductDTO> validator)
        {
            CreateProductDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToProductDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateProductDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid product payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new CreateProductCommand(dto);
            var productDTO = await mediator.Send(command);
            return productDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<IResult> Update(
            Guid id,
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateProductDTO> validator)
        {
            CreateProductDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToProductDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateProductDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid product payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new UpdateProductCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<IResult> UploadPicture(
            Guid id,
            [FromForm] SITRA.Core.DTOs.Products.UploadPictureDTO dto,
            [FromServices] FluentValidation.IValidator<SITRA.Core.DTOs.Products.UploadPictureDTO> validator,
            [FromServices] SITRA.Core.Interfaces.IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var file = dto.GetFile()!;
            var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
            if (product == null)
                return TypedResults.NotFound();

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await unitOfWork.DgiiStorageService.DeleteItemPictureAsync(product.ImageUrl);
            }

            string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
            var url = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(file, rnc, id);
            product.ImageUrl = url;
            unitOfWork.ProductRepository.Update(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(new { imageUrl = url });
        }

        private static CreateProductDTO FormToProductDTO(IFormCollection form)
        {
            var taxIds = form["taxIds"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
            if (!taxIds.Any() && form.ContainsKey("taxIds[]"))
            {
                taxIds = form["taxIds[]"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                    .Where(g => g != Guid.Empty).ToList();
            }

            return new CreateProductDTO
            {
                Name = form["name"].ToString(),
                Reference = string.IsNullOrWhiteSpace(form["reference"]) ? null : form["reference"].ToString(),
                Barcode = string.IsNullOrWhiteSpace(form["barcode"]) ? null : form["barcode"].ToString(),
                Description = string.IsNullOrWhiteSpace(form["description"]) ? null : form["description"].ToString(),
                InitialCost = double.TryParse(form["initialCost"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ic) ? ic : 0,
                BasePrice = double.TryParse(form["basePrice"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var bp) ? bp : 0,
                UnitMeasure = Enum.TryParse<UnitMeasure>(form["unitMeasure"], out var um) ? um : null,
                BrandId = Guid.TryParse(form["brandId"], out var bi) ? bi : null,
                ModelId = Guid.TryParse(form["modelId"], out var mi) ? mi : null,
                CategoryId = Guid.TryParse(form["categoryId"], out var ci) ? ci : null,
                SubCategoryId = Guid.TryParse(form["subCategoryId"], out var sci) ? sci : null,
                WarehouseId = Guid.TryParse(form["warehouseId"], out var wi) ? wi : null,
                TaxIds = taxIds,
                ImageUrl = form.Files["imageUrl"] ?? form.Files["image"] ?? form.Files["file"] ?? form.Files.FirstOrDefault()
            };
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateProductCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateProductCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        public static async Task<Results<Ok, ValidationProblem>> UploadCsv(
            [FromForm] CsvDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UploadCsvProductCommand(dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { "An error occurred while processing the CSV." } }
            });
        }
    }
}

