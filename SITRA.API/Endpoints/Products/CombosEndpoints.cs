using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.Mediator.Commands.Products.Combos;
using SITRA.Core.Mediator.Queries.Products.Combos;

namespace SITRA.API.Endpoints
{
    public static class CombosEndpoints
    {
        public static RouteGroupBuilder MapCombos(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("combos-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPost("/{id:guid}/picture", UploadPicture).DisableAntiforgery()
                .WithOpenApi()
                .RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ComboListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllCombosQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ComboDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetComboByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<IResult> Create(
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateComboDTO> validator)
        {
            CreateComboDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToComboDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateComboDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid combo payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new CreateComboCommand(dto);
            var productDTO = await mediator.Send(command);
            return productDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<IResult> Update(
            Guid id,
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateComboDTO> validator)
        {
            CreateComboDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToComboDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateComboDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid combo payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new UpdateComboCommand(id, dto);
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
            var combo = await unitOfWork.ComboRepository.GetByIdAsync(id);
            if (combo == null)
                return TypedResults.NotFound();

            if (!string.IsNullOrEmpty(combo.ImageUrl))
            {
                await unitOfWork.DgiiStorageService.DeleteItemPictureAsync(combo.ImageUrl);
            }

            string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
            var url = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(file, rnc, id);
            combo.ImageUrl = url;
            unitOfWork.ComboRepository.Update(combo);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(new { imageUrl = url });
        }

        private static CreateComboDTO FormToComboDTO(IFormCollection form)
        {
            var taxIds = form["taxIds"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
            if (!taxIds.Any() && form.ContainsKey("taxIds[]"))
            {
                taxIds = form["taxIds[]"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                    .Where(g => g != Guid.Empty).ToList();
            }

            var items = new List<ItemQtyDTO>();
            var itemsJson = form["items"].ToString();
            if (!string.IsNullOrWhiteSpace(itemsJson))
            {
                try
                {
                    var parsed = System.Text.Json.JsonSerializer.Deserialize<List<ItemQtyDTO>>(
                        itemsJson,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (parsed != null) items = parsed;
                }
                catch { }
            }

            return new CreateComboDTO
            {
                Name = form["name"].ToString(),
                Reference = string.IsNullOrWhiteSpace(form["reference"]) ? null : form["reference"].ToString(),
                Barcode = string.IsNullOrWhiteSpace(form["barcode"]) ? null : form["barcode"].ToString(),
                Description = string.IsNullOrWhiteSpace(form["description"]) ? null : form["description"].ToString(),
                InitialCost = double.TryParse(form["initialCost"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ic) ? ic : 0,
                BasePrice = double.TryParse(form["basePrice"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var bp) ? bp : 0,
                UnitMeasure = Enum.TryParse<SITRA.Core.Enums.UnitMeasure>(form["unitMeasure"], out var um) ? um : null,
                BrandId = Guid.TryParse(form["brandId"], out var bi) ? bi : null,
                ModelId = Guid.TryParse(form["modelId"], out var mi) ? mi : null,
                CategoryId = Guid.TryParse(form["categoryId"], out var ci) ? ci : null,
                SubCategoryId = Guid.TryParse(form["subCategoryId"], out var sci) ? sci : null,
                TaxIds = taxIds,
                Items = items,
                ImageUrl = form.Files["imageUrl"] ?? form.Files["image"] ?? form.Files["file"] ?? form.Files.FirstOrDefault()
            };
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateComboCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateComboCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }
    }
}

