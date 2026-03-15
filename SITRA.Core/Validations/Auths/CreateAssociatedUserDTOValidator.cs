using FluentValidation;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using SITRA.Core.Entities.Tenants;

namespace SITRA.Core.Validations.Auths
{
    public class CreateAssociatedUserDTOValidator
        : AbstractValidator<CreateAssociatedUserDTO>
    {
        public CreateAssociatedUserDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .EmailAddress().WithMessage(Utilities.EmailMessage)
                .MustAsync(async (email, cancellation) =>
                {
                    bool exists = await unitOfWork.UserService.ExistsByEmailAsync(email);
                    return !exists;
                })
                .WithMessage(x => Utilities.EmailAlreadyExistsMessage(x.Email))
                .WithName("Email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(6).WithMessage(Utilities.PasswordMinLengthMessage);

            RuleFor(x => x)
                .CustomAsync(async (dto, context, cancellation) =>
                {
                    var user = await unitOfWork.UserService.GetUserAsync();
                    if (user == null) return;

                    var tenant = await unitOfWork.Repository<Tenant>().GetByIdAsync(user.TenantId, cancellation);
                    if (tenant == null) return;

                    var maxUsers = unitOfWork.PlanService.GetMaxUsers(tenant.Plan);
                    
                    var associatedUsers = await unitOfWork.UserService.GetAllAssociatedUsersAsync(cancellation);
                    
                    if (associatedUsers.Count + 1 >= maxUsers)
                    {
                        context.AddFailure("PlanLimit", Utilities.MaximumUsersReachedMessage(maxUsers, tenant.Plan.ToString()));
                    }
                });
        }
    }
}


