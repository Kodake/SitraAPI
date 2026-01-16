using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateFiscalSupportDTOValidator : AbstractValidator<CreateFiscalSupportDTO>
    {
        public CreateFiscalSupportDTOValidator(
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            long id = 0;

            if (routeId is string idString)
            {
                long.TryParse(idString, out id);
            }

            RuleFor(x => x)
                .MustAsync(async (dto, _) =>
                {
                    var existing = await unitOfWork.FiscalSupportRepository
                        .GetByTypeAndWarehouseAsync(dto.FiscalSupportType, dto.WarehouseId);

                    if (existing == null) return true;

                    if (id > 0 && existing.Id == id) return true;

                    return false;
                })
                .WithMessage(dto => Utilities.FiscalSupportAlreadyExistsMessage(dto.FiscalSupportType, dto.WarehouseId));
        }
    }
}


