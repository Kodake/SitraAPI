using FluentValidation;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;

namespace SITRA.Core.Validations.AdministrativeTools
{
    public class AssignPermissionsDTOValidator : AbstractValidator<AssignPermissionsDTO>
    {
        public AssignPermissionsDTOValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage);

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage(Utilities.PermissionIdsRequiredMessage);
        }
    }
}
