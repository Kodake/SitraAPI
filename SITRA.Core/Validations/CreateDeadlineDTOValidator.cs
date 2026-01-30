using FluentValidation;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.AdministrativeTools;

namespace SITRA.Core.Validations
{
    public class CreateDeadlineDTOValidator : AbstractValidator<CreateDeadlineDTO>
    {
        public CreateDeadlineDTOValidator(IDeadlineRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.Days)
            .NotNull().WithMessage(Utilities.RequiredFieldMessage)
            .GreaterThanOrEqualTo(0).WithMessage(Utilities.GreatherThanZero)
            .MustAsync(async (days, _) =>
            {
                var exists = await repository.Exists(id, days);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Days));
        }
    }
}


