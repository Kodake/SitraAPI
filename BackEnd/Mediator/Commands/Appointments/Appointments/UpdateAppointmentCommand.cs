using AutoMapper;
using BackEnd.DTOs.Appointments.Appointments;
using BackEnd.Entities.Appointments;
using BackEnd.Repositories;
using Microsoft.AspNetCore.OutputCaching;

namespace BackEnd.Mediator.Commands.Appointments.Appointments
{
    public class UpdateAppointmentCommand : UpdateCommand<Appointment, CreateAppointmentDTO>
    {
        public UpdateAppointmentCommand(long id, CreateAppointmentDTO dto) : base(id, dto) { }

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
