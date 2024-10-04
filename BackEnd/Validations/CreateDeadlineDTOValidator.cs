using BackEnd.DTOs.AdministrativeTools;
using BackEnd.Helpers;
using BackEnd.Repositories.AdministrativeTools;
using FluentValidation;

namespace BackEnd.Validations
{
    public class CreateDeadlineDTOValidator : AbstractValidator<CreateDeadlineDTO>
    {
        public CreateDeadlineDTOValidator(IDeadlineRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            RuleFor(x => x.Days)
            .NotNull().WithMessage(Utilities.RequiredFieldMessage)
            .GreaterThanOrEqualTo(0).WithMessage("El valor de días no puede ser negativo")
            .MustAsync(async (days, _) =>
            {
                var existe = await repository.Exists(id, days);
                return !existe;
            }).WithMessage(x => $"Ya existe un plazo con {x.Days} días");
        }
    }
}
