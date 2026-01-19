using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;
using System.Linq;

namespace SITRA.Core.Validations
{
    public class CreateAppointmentDTOValidator : AbstractValidator<CreateAppointmentDTO>
    {
        public CreateAppointmentDTOValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.ContactId)
                .MustAsync(async (id, cancellation) => await unitOfWork.ContactRepository.ExistsAsync(id))
                .WithMessage(Utilities.ContactNotFoundOrInvalidMessage);

            RuleFor(x => x.ScheduledAt)
                .NotNull().WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (date, cancellation) => !await unitOfWork.AppointmentRepository.ExistsAtDateTime(date))
                .WithMessage(Utilities.AppointmentAlreadyExist);

            RuleFor(x => x.AppointmentItemIds)
                .NotEmpty().WithMessage(Utilities.AtLeastOneServiceToChose)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage(Utilities.DuplicateServicesMessage)
                .MustAsync(async (ids, cancellation) =>
                {
                    if (ids == null || !ids.Any()) return true;
                    foreach (var id in ids)
                    {
                        var exists = await unitOfWork.ServiceRepository.ExistsAsync(id);
                        if (!exists) return false;
                    }
                    return true;
                })
                .WithMessage(Utilities.OneOrMoreServicesInvalidMessage);
        }
    }
}
