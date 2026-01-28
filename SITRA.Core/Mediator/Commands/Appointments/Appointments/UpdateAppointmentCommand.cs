using AutoMapper;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace SITRA.Core.Mediator.Commands.Appointments.Appointments
{
    public class UpdateAppointmentCommand : UpdateCommand<Appointment, CreateAppointmentDTO>
    {
        public UpdateAppointmentCommand(Guid id, CreateAppointmentDTO dto) : base(id, dto) { }

        public override async Task<bool> ExecuteAction(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper, CancellationToken cancellationToken)
        {
            var appointmentDB = await unitOfWork.AppointmentRepository.GetByIdAsync(Id);

            if (appointmentDB == null) return false;

            mapper.Map(Dto, appointmentDB);

            appointmentDB.AppointmentItems = Dto.AppointmentItemIds.Select(itemId => new AppointmentItem { ItemId = itemId }).ToList();

            unitOfWork.AppointmentRepository.Update(appointmentDB);
            await unitOfWork.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync("appointments-get", default);

            return true;
        }
    }
}
