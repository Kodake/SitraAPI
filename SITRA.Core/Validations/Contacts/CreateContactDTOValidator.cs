using FluentValidation;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces.Contacts;

namespace SITRA.Core.Validations.Contacts
{
    public class CreateContactDTOValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactDTOValidator(IContactRepository repository,
            IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = Guid.Empty;

            if (routeId is string valorString)
            {
                _ = Guid.TryParse(valorString, out id);
            }

            RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(50).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage)
            .MustAsync(async (code, _) =>
            {
                var exists = await repository.Exists(id, code);
                return !exists;
            }).WithMessage(x => string.Format(Utilities.NameAlreadyExistsWith, x.Name));

            RuleFor(x => x.Province)
            .MaximumLength(2).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

            RuleFor(x => x.Identification)
            .MaximumLength(15).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);

            RuleFor(x => x.ContactType)
            .Must(value => value != null && Enum.IsDefined(typeof(ContactType), value))
            .WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.IdentificationType)
            .Must(value => value != null && Enum.IsDefined(typeof(IdentificationType), value))
            .WithMessage(Utilities.InvalidEnumValueMessage);

            RuleFor(x => x.Address)
            .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
            .MinimumLength(2).WithMessage(Utilities.MinimunLengthMessage)
            .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage)
            .Must(Utilities.FirstCharUppercase).WithMessage(Utilities.FirstCharUpperMessage);
        }
    }
}


