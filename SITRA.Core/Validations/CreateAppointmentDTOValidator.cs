using Microsoft.AspNetCore.Http;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Enums;
using SITRA.Core.Helpers;
using SITRA.Core.Interfaces;
using FluentValidation;

namespace SITRA.Core.Validations
{
    public class CreateAppointmentDTOValidator
        : AbstractValidator<CreateAppointmentDTO>
    {
        public CreateAppointmentDTOValidator(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            // ?? routeId (MISMO patrón que usas siempre)
            var routeId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            // Description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MinimumLength(5).WithMessage(Utilities.MinimunLengthMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            //// ContactId
            //RuleFor(x => x.ContactId)
            //    .GreaterThan(0).WithMessage(Utilities.RequiredFieldMessage)
            //    .MustAsync(async (contactId, _) =>
            //        await unitOfWork.ContactRepository.Exists(contactId))
            //    .WithMessage(Utilities.NotExistsWith);

            //RuleFor(x => x.ScheduledAt)
            //    .MustAsync(async (scheduledAt, _) =>
            //    {
            //        var exists = await unitOfWork.AppointmentRepository
            //            .ExistsAtDateTime(scheduledAt);

            //        return !exists;
            //    })
            //    .WithMessage(Utilities.AppointmentAlreadyExist);


            //// AppointmentItemIds
            //RuleFor(x => x.AppointmentItemIds)
            //    .NotNull().WithMessage(Utilities.RequiredFieldMessage)
            //    .Must(x => x.Count > 0)
            //    .WithMessage(Utilities.AtLeastOneServiceToChose);

            //RuleForEach(x => x.AppointmentItemIds)
            //    .GreaterThan(0).WithMessage(Utilities.RequiredFieldMessage)
            //    .MustAsync(async (itemId, _) =>
            //        await unitOfWork.ItemRepository.Exists(itemId, ItemType.Service))
            //    .WithMessage(Utilities.NotDuplicatesWith);

            //// Evitar duplicados
            //RuleFor(x => x.AppointmentItemIds)
            //    .Must(x => x.Distinct().Count() == x.Count)
            //    .WithMessage(Utilities.NotDuplicatesWith);

            // ContactId
            RuleFor(x => x.ContactId)
                .GreaterThan(0).WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (contactId, _) =>
                    await unitOfWork.ContactRepository.Exists(contactId))
                .WithMessage("El contacto seleccionado no existe o no es válido.");

            RuleFor(x => x.ScheduledAt)
                .MustAsync(async (scheduledAt, _) =>
                {
                    var exists = await unitOfWork.AppointmentRepository
                        .ExistsAtDateTime(scheduledAt, id == 0 ? null : id);

                    return !exists;
                })
                .WithMessage("Ya existe una cita programada para esta fecha y hora.");


            // AppointmentItemIds
            RuleFor(x => x.AppointmentItemIds)
                .NotNull().WithMessage(Utilities.RequiredFieldMessage)
                .Must(x => x.Count > 0)
                .WithMessage("Debe seleccionar al menos un servicio.");

            RuleForEach(x => x.AppointmentItemIds)
                .GreaterThan(0).WithMessage(Utilities.RequiredFieldMessage)
                .MustAsync(async (itemId, _) =>
                    await unitOfWork.ItemRepository.Exists(itemId, ItemType.Service))
                .WithMessage("Uno o más servicios seleccionados no existen o no son válidos.");

            // Evitar duplicados
            RuleFor(x => x.AppointmentItemIds)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("No se permiten servicios duplicados.");
        }
    }
}


