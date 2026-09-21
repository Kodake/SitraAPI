using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SITRA.API.Filters;
using SITRA.API.Helpers;
using SITRA.Core.DTOs;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.Enums;
using SITRA.Core.Mediator.Commands.Products.Services;
using SITRA.Core.Mediator.Queries.Products.Services;

namespace SITRA.API.Endpoints.Products
{
    public static class ServicesEndpoints
    {
        public static RouteGroupBuilder MapServices(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get"))
                .AddPaginationParamsOpenAPI().RequireAuthorization();
            group.MapGet("/selects", GetSelects).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("services-get")).RequireAuthorization();
            group.MapGet("/{id:guid}", GetById).RequireAuthorization();
            group.MapPost("/", Create).DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();
            group.MapPut("/{id:guid}", Update).DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();
            group.MapPost("/{id:guid}/picture", UploadPicture).DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();
            group.MapPatch("/{id:guid}/activate", Activate).RequireAuthorization();
            group.MapDelete("/{id:guid}/deactivate", Deactivate).RequireAuthorization();
            group.MapPost("/upload-csv", UploadCsv).DisableAntiforgery()
                .WithOpenApi().RequireAuthorization();
            return group;
        }

        static async Task<Ok<List<ServiceListDTO>>> GetAll(
            [FromServices] IMediator mediator,
            PaginationDTO pagination)
        {
            var query = new GetAllServicesQuery(pagination);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Ok<List<ServiceListDTO>>> GetSelects(
            [FromServices] IMediator mediator,
            [FromQuery] StatusType status = StatusType.All)
        {
            var query = new GetSelectsServicesQuery(status);
            var dto = await mediator.Send(query);
            return TypedResults.Ok(dto);
        }

        static async Task<Results<Ok<ServiceDTO>, NotFound>> GetById(
            [FromServices] IMediator mediator,
            Guid id)
        {
            var query = new GetServiceByIdQuery(id);
            var dto = await mediator.Send(query);
            return dto == null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        }

        static async Task<IResult> Create(
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateServiceDTO> validator)
        {
            CreateServiceDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToServiceDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateServiceDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid service payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new CreateServiceCommand(dto);
            var serviceDTO = await mediator.Send(command);
            return serviceDTO == null ? TypedResults.NotFound() : TypedResults.Created();
        }

        static async Task<IResult> Update(
            Guid id,
            HttpContext httpContext,
            [FromServices] IMediator mediator,
            [FromServices] FluentValidation.IValidator<CreateServiceDTO> validator)
        {
            CreateServiceDTO? dto;
            if (httpContext.Request.HasFormContentType)
            {
                var form = await httpContext.Request.ReadFormAsync();
                dto = FormToServiceDTO(form);
            }
            else
            {
                dto = await httpContext.Request.ReadFromJsonAsync<CreateServiceDTO>();
            }

            if (dto == null)
                return TypedResults.Problem("Invalid service payload.");

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            var command = new UpdateServiceCommand(id, dto);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<IResult> UploadPicture(
            Guid id,
            IFormFile file,
            [FromServices] SITRA.Core.Interfaces.IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return TypedResults.BadRequest("No file provided.");

            var service = await unitOfWork.ServiceRepository.GetByIdAsync(id);
            if (service == null)
                return TypedResults.NotFound();

            if (!string.IsNullOrEmpty(service.ImageUrl))
            {
                await unitOfWork.DgiiStorageService.DeleteItemPictureAsync(service.ImageUrl);
            }

            string rnc = await SITRA.Core.Helpers.TenantFiscalSupportHelper.GetCurrentTenantRncAsync(
                unitOfWork, unitOfWork.HttpContextAccessor, cancellationToken);
            var url = await unitOfWork.DgiiStorageService.SaveItemPictureAsync(file, rnc, id);
            service.ImageUrl = url;
            unitOfWork.ServiceRepository.Update(service);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(new { imageUrl = url });
        }

        private static CreateServiceDTO FormToServiceDTO(IFormCollection form)
        {
            var taxIds = form["taxIds"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty).ToList();
            if (!taxIds.Any() && form.ContainsKey("taxIds[]"))
            {
                taxIds = form["taxIds[]"].Select(t => Guid.TryParse(t, out var g) ? g : Guid.Empty)
                    .Where(g => g != Guid.Empty).ToList();
            }

            return new CreateServiceDTO
            {
                Name = form["name"].ToString(),
                Reference = string.IsNullOrWhiteSpace(form["reference"]) ? null : form["reference"].ToString(),
                Description = string.IsNullOrWhiteSpace(form["description"]) ? null : form["description"].ToString(),
                InitialCost = double.TryParse(form["initialCost"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ic) ? ic : 0,
                BasePrice = double.TryParse(form["basePrice"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var bp) ? bp : 0,
                UnitMeasure = Enum.TryParse<UnitMeasure>(form["unitMeasure"], out var um) ? um : default,
                CategoryId = Guid.TryParse(form["categoryId"], out var ci) ? ci : null,
                SubCategoryId = Guid.TryParse(form["subCategoryId"], out var sci) ? sci : null,
                TaxIds = taxIds,
                ImageUrl = form.Files["imageUrl"] ?? form.Files["image"] ?? form.Files["file"] ?? form.Files.FirstOrDefault()
            };
        }

        static async Task<Results<NotFound, Ok>> Activate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new ActivateServiceCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        static async Task<Results<NotFound, Ok>> Deactivate(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator)
        {
            var command = new DeactivateServiceCommand(id);
            var success = await mediator.Send(command, default);
            return success ? TypedResults.Ok() : TypedResults.NotFound();
        }

        public static async Task<Results<Ok, ValidationProblem>> UploadCsv(
            [FromForm] CsvDTO dto,
            [FromServices] IMediator mediator)
        {
            var command = new UploadCsvServiceCommand(dto);
            var success = await mediator.Send(command);
            return success ? TypedResults.Ok() : TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { "An error occurred while processing the CSV." } }
            });
        }
    }
}

