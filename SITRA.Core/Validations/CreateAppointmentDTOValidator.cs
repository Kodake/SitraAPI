using FluentValidation;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Validations
{
    public class CreateAppointmentDTOValidator : AbstractValidator<CreateAppointmentDTO>
    {
        public CreateAppointmentDTOValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            Guid id = Guid.Empty;
            if (routeId != null)
            {
                Guid.TryParse(routeId.ToString(), out id);
            }

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.ContactId)
                .MustAsync(async (contactId, cancellation) => await unitOfWork.ContactRepository.ExistsAsync(contactId))
                .WithMessage(Utilities.ContactNotFoundOrInvalidMessage);

            RuleFor(x => x.ScheduledAt)
                .NotNull().WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (dto, date, cancellation) =>
                    !await unitOfWork.AppointmentRepository
                        .ExistsAtDateTime(date, id != Guid.Empty ? id : null))
                .WithMessage(Utilities.AppointmentAlreadyExist);

            RuleFor(x => x.AppointmentItemIds)
                .NotEmpty().WithMessage(Utilities.AtLeastOneServiceToChose)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage(Utilities.DuplicateServicesMessage)
                .MustAsync(async (ids, cancellation) =>
                {
                    if (ids == null || !ids.Any()) return true;
                    foreach (var itemId in ids)
                    {
                        var exists = await unitOfWork.ServiceRepository.ExistsAsync(itemId);
                        if (!exists) return false;
                    }
                    return true;
                })
                .WithMessage(Utilities.OneOrMoreServicesInvalidMessage);
        }
    }
}
